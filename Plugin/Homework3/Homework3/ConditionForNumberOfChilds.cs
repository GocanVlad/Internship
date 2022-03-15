using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Homework3
{
    public class ConditionForNumberOfChilds : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity entity = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    Guid tshirtId = entity.Id;
                    var lookupColumns = new ColumnSet("aug_client");
                    var retrievedTshirt = service.Retrieve("aug_tshirt", tshirtId, lookupColumns);
                    Guid clientId = ((EntityReference)retrievedTshirt.Attributes["aug_client"]).Id;                  

                    var lookupQuery = new QueryExpression("aug_tshirt");
                    lookupQuery.Criteria.AddCondition(new ConditionExpression("aug_client", ConditionOperator.Equal, clientId));
                    lookupQuery.ColumnSet = new ColumnSet(true);
                    
                    var tshirtResults = service.RetrieveMultiple(lookupQuery);
                    var counter = 0;
                    if(tshirtResults.Entities.Any())
                    {
                        foreach (var child in tshirtResults.Entities)
                        {
                            if (child.Attributes.TryGetValue("aug_brand", out object brandObj))
                            {
                                var brand = brandObj.ToString();
                                if (brand.Equals("brand 2"))
                                    counter++;
                            }
                        }
                    }
                    var client = new Entity("account", clientId);
                    client.Attributes.Add("aug_dealswithbrand2", counter.ToString());
                    service.Update(client);
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
