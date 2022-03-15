using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using CsvHelper;
using System.IO;
using System.Globalization;
using CsvHelper.Configuration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using CsvHelper.Configuration.Attributes;

namespace ConsoleAppH3
{

    class Program
    {
        private static readonly Dictionary<string, string> tshirts = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            tshirts.Add("aug_name", "Name");
            tshirts.Add("aug_client", "Client");
            tshirts.Add("aug_size", "Size");
            tshirts.Add("aug_quantity", "Quantity");
            tshirts.Add("aug_price", "Unit Price");
            tshirts.Add("aug_tva", "TVA");
            tshirts.Add("aug_expecteddelivery", "Expected Delivery");

            string connectionString = ConfigurationManager.ConnectionStrings["MyGocServer"].ConnectionString;
            using (var service = new CrmServiceClient(connectionString))
            {
                bool stop = false;
                do
                {
                    Console.WriteLine("Pick an action, type any thing else to quit.");
                    Console.WriteLine("1 Import to Dataverse from CSV");
                    Console.WriteLine("2 Export to CSV from Dataverse");
                    Console.WriteLine("3 pentru - din CSV in Dataverse + din Dataverse in CSV");

                    string action = Console.ReadLine();

                    switch (action)
                    {
                        case "1":
                            try
                            {
                                ImportFromCSVtoDataverse(service);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error importing from CSV to Dataverse: " + ex.Message);
                            }
                            break;
                        case "2":
                            try
                            {
                                ExportToCSVFromDataverse(service);
                            }
                            catch (Exception ex) { Console.WriteLine("Error exporting from Dateverse to CSV: " + ex.Message); }
                            break;
                        case "3":
                            try
                            {
                                ImportAndExportFromDataverse(service);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error: " + ex.Message);
                            }
                            break;
                        default:
                            stop = true;
                            break;
                    }
                } while (!stop);
                Console.WriteLine("Press any key to exit.");
            }
        }
        private static object GetClientEntityReferenceByName(CrmServiceClient service, string clientName)
        {
            var accountClientQuery = new QueryExpression
            {
                EntityName = "account",
                Criteria = new FilterExpression
                {
                    Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = "name",
                                Operator = ConditionOperator.Equal,
                                Values = { clientName }
                            }
                        }
                },
            };

            Entity clientEntity = service.RetrieveMultiple(accountClientQuery).Entities.Select(result => result).FirstOrDefault();
            return new EntityReference("account", clientEntity.Id);
        }
        private static EntityCollection GetEntityCollection(CrmServiceClient service, string entityName, string attributeName, string attributeValue, ColumnSet cols)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = entityName,
                ColumnSet = cols,
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = attributeName,
                            Operator = ConditionOperator.Equal,
                            Values = { attributeValue }
                        }
                    }
                }
            };
            return service.RetrieveMultiple(query);
        }
        public static int GetOptionSetValueFromLabel(CrmServiceClient service, string optionsetText)
        {
            var attributeRequest = new RetrieveAttributeRequest()
            {
                EntityLogicalName = "aug_tshirt",
                LogicalName = "aug_size"
            };


            var attributeResponse = (RetrieveAttributeResponse)service.Execute(attributeRequest);
            var picklistAttributeMetadata = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;

            OptionSetMetadata optionsetMetadata = picklistAttributeMetadata.OptionSet;
            var optionList = (from o in optionsetMetadata.Options
                              select new { o.Value, Text = o.Label.UserLocalizedLabel.Label }).ToList();

            var optionSetValue = optionList.Where(o => o.Text.Trim() == optionsetText.Trim())
            .Select(o => o.Value)
            .FirstOrDefault();

            return (int)optionSetValue;
        }
        public static string GetOptionSetLabel(CrmServiceClient service, int value)
        {
            var attributeReq = new RetrieveAttributeRequest
            {
                EntityLogicalName = "aug_tshirt",
                LogicalName = "aug_size",
                RetrieveAsIfPublished = true
            };

            var attributeResp = (RetrieveAttributeResponse)service.Execute(attributeReq);
            var attributeMetadata = (EnumAttributeMetadata)attributeResp.AttributeMetadata;
            var optionList = (from o in attributeMetadata.OptionSet.Options
                              select new { o.Value, Text = o.Label.UserLocalizedLabel.Label }).ToList();

            var activeValue = optionList.Where(o => o.Value == value)
                                      .Select(o => o.Text)
                                      .FirstOrDefault();
            return activeValue;
        }
        private static EntityCollection RetrieveDataFromDataverse(CrmServiceClient service)
        {
            var columnSet = new ColumnSet();
            columnSet.AddColumns(tshirts.Keys.ToArray());

            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = "aug_tshirt",
                ColumnSet = columnSet
            };

            EntityCollection entityCollection = service.RetrieveMultiple(query);
            return entityCollection;
        }
        private static void CreateEntity(CrmServiceClient service, Tshirt item)
        {
            Entity augTshirt = new Entity("aug_tshirt");
            augTshirt.Attributes["aug_name"] = item.Name;
            augTshirt.Attributes["aug_client"] = GetClientEntityReferenceByName(service, item.Client);
            augTshirt.Attributes.Add("aug_size", new OptionSetValue(GetOptionSetValueFromLabel(service, item.Size)));
            augTshirt.Attributes["aug_quantity"] = Convert.ToInt32(item.Quantity);
            augTshirt.Attributes["aug_price"] = new Money(Convert.ToInt32(item.UnitPrice));
            augTshirt.Attributes["aug_tva"] = Convert.ToDouble(item.TVA);
            augTshirt.Attributes["aug_expecteddelivery"] = Convert.ToDateTime(item.ExpectedDelivery);
            augTshirt.Attributes["aug_brand"] = item.Brand;
            service.Create(augTshirt);
        }
        private static void UpdateEntityByName(CrmServiceClient service, CsvReader csv)
        {
            var records = csv.GetRecords<Tshirt>().ToList();
            EntityCollection entityCollection = RetrieveDataFromDataverse(service);
            foreach (var entity in entityCollection.Entities)
            {
                var entityNameAttr = entity.Attributes["aug_name"];
                foreach (var item in records)
                {
                    if ((string)entityNameAttr == item.Name)
                    {
                        entity.Attributes["aug_client"] = GetClientEntityReferenceByName(service, item.Client);
                        entity.Attributes["aug_size"] = new OptionSetValue(GetOptionSetValueFromLabel(service, item.Size));
                        entity.Attributes["aug_quantity"] = Convert.ToInt32(item.Quantity);
                        entity.Attributes["aug_price"] = new Money(Convert.ToInt32(item.UnitPrice));
                        entity.Attributes["aug_tva"] = Convert.ToDouble(item.TVA);
                        entity.Attributes["aug_expecteddelivery"] = Convert.ToDateTime(item.ExpectedDelivery);
                        entity.Attributes["aug_brand"] = item.Brand;

                        service.Update(entity);
                    }
                }
            }
        }
        private static void DeleteEntityByCSVName(CrmServiceClient service, CsvReader csv)
        {
            csv.Read();
            csv.ReadHeader();
            var records = csv.GetRecords<Tshirt>().ToList();
            EntityCollection entityCollection = RetrieveDataFromDataverse(service);
            foreach (var entity in entityCollection.Entities)
            {
                foreach (var csvRecord in records)
                {
                    var entityId = entity.Id;
                    var entityNameAttr = entity.Attributes["aug_name"];
                    if ((string)entityNameAttr == csvRecord.Name)
                        service.Delete("aug_tshirt", entityId);
                }
            }
        }
        private static Guid CheckIfRecordExistsByName(CrmServiceClient service, Tshirt item, out object name)
        {
            name = null;
            var entity = GetEntityCollection(service, "aug_tshirt", "aug_name",item.Name, new ColumnSet("aug_name"));
            Guid entityId =new Guid();
            foreach(var x in entity.Entities)
            {
                name = x.Attributes["aug_name"];
                entityId = x.Id;
            }

            return entityId;
        }
        private static void ImportFromCSVtoDataverse(CrmServiceClient service)
        {
            

            string path = "C://Users//Z004ES4D//Desktop//Plugin//CsvFiles//";
            DirectoryInfo csvDir = new DirectoryInfo(path);
            FileInfo[] files = csvDir.GetFiles("*.csv");

            Console.WriteLine("Found files");
            foreach (FileInfo file in files)
            {
                Console.WriteLine(file.Name);
            }

            Console.Write("Type file name: ");
            string filePath = csvDir + Convert.ToString(Console.ReadLine()) + ".csv";

            Console.Write("Type delimiter: ");
            string delimiter = Convert.ToString(Console.ReadLine());

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = delimiter
            };

            var streamReader = File.OpenText(filePath);
            CsvReader csv = new CsvReader(streamReader, csvConfiguration);
            csv.Read();
            csv.ReadHeader();

            var records = csv.GetRecords<Tshirt>().ToList();
            foreach (var item in records)
                CreateEntity(service, item);

            Console.WriteLine("CSV file imported succesfully!");
            Console.WriteLine();
            streamReader.Close();
        }      
        private static void ExportToCSVFromDataverse(CrmServiceClient service)
        {
            string path = "C://Users//Z004ES4D//Desktop//Plugin//CsvFiles//ExportsFromDatavers.csv";
            string delimiter = ",";
            
            EntityCollection entityCollection = RetrieveDataFromDataverse(service);

            StreamWriter sw = new StreamWriter(path, false);
            sw.Write(string.Join(delimiter, tshirts.Select(x => x.Value).ToArray()));
            sw.Write(sw.NewLine);

            foreach (Entity entity in entityCollection.Entities)
            {
                var newLine = "";
                foreach (var field in tshirts)
                {
                    if (!entity.Contains(field.Key))
                    {
                        newLine += delimiter;
                        continue;
                    }
                    var value = entity[field.Key];

                    if (value is EntityReference valueEntityReference)
                    {
                        newLine += valueEntityReference.Name;
                    }
                    else if (value is OptionSetValue valueOptionSetValue)
                    {
                        newLine += GetOptionSetLabel(service, valueOptionSetValue.Value);
                    }
                    else if (value is Money valueMoney)
                    {
                        newLine += valueMoney.Value;
                    }
                    else
                    {
                        newLine += value.ToString();
                    }
                    newLine += delimiter;
                }
                sw.Write(newLine);
                sw.Write(sw.NewLine);
            }
            sw.Close();
            Console.WriteLine("Data exported to: " + path);
            Console.WriteLine();
        }
        private static void ImportAndExportFromDataverse(CrmServiceClient service)
        {
            var filePath = "C://Users//Z004ES4D//Desktop//Plugin//CsvFiles//data_to_import1.csv";
            var filePathU = "C://Users//Z004ES4D//Desktop//Plugin//CsvFiles//data_to_import2.csv";
            var filePath1 = "C://Users//Z004ES4D//Desktop//Plugin//CsvFiles//data_to_import3.csv";

            var streamReader = File.OpenText(filePath);
            var streamReaderU = File.OpenText(filePathU);
            var streamReader1 = File.OpenText(filePath1);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            };
            CsvReader csv = new CsvReader(streamReader, csvConfiguration);
            CsvReader csvU = new CsvReader(streamReaderU, csvConfiguration);
            CsvReader csv1 = new CsvReader(streamReader1, csvConfiguration);
            var records = csv.GetRecords<Tshirt>().ToList();
            foreach (var item in records)
                CheckIfRecordExistsByName(service, item, out object name);
            UpdateEntityByName(service, csvU);
            DeleteEntityByCSVName(service, csv1);
            Console.WriteLine("Dataverse data has been updated succesfully!");
            ExportToCSVFromDataverse(service);
        }
        public class Tshirt
        {
            [Name("Name")]
            public string Name { get; set; }
            [Name("Client")]
            public string Client { get; set; }
            [Name("Size")]
            public string Size { get; set; }
            [Name("Quantity")]
            public int Quantity { get; set; }
            [Name("Unit Price")]
            public int UnitPrice { get; set; }
            [Name("TVA")]
            public double TVA { get; set; }
            [Name("Expected Delivery")]
            public DateTime ExpectedDelivery { get; set; }
            [Name("Brand")]
            public string Brand { get; set; }

        }
        public class TshirtClassMap: ClassMap<Tshirt>
        {
            public TshirtClassMap()
            {
                Map(m => m.Name).Name("Name");
                Map(m => m.Client).Name("Client");
                Map(m => m.Size).Name("Size");
                Map(m => m.Quantity).Name("Quantity");
                Map(m => m.UnitPrice).Name("Unit Price");
                Map(m => m.TVA).Name("TVA");
                Map(m => m.ExpectedDelivery).Name("Expected Delivery");
                Map(m => m.Brand).Name("Brand");
            }
        }
        
    }
}

