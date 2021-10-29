using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopTenPops
{
    class CsvReader
    {
        private string _csvFilePath;
        public CsvReader(string csvFilePath)
        {
            this._csvFilePath = csvFilePath;
        }

        public Country[] ReadFirstNCountries(int nCounties)
        {
            
            Country[] countries = new Country[nCounties];
            using (StreamReader sr = new StreamReader(_csvFilePath))
            {
                sr.ReadLine();
                for(int i=0;i<nCounties;i++)
                {
                    string csvLine = sr.ReadLine();
                    countries[i] = ReadCountryFormCsvFile(csvLine);
                }
            }
                return countries;
        }

        public Country ReadCountryFormCsvFile(string csvLine)
        {
            string[] parts = csvLine.Split(new char[] { ',' });

            string name = parts[0];
            string code = parts[1];
            string region = parts[2];
            int population = int.Parse(parts[3]);

            return new Country(name, code, region, population);
        }
    }
}
