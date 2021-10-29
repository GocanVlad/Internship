using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadAllCountries
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\Z004ES4D\Internship\T3\TopTenPops\Pop by Largest Final.csv";
            CsvReader reader = new CsvReader(filePath);

            List<Country> countries = reader.ReadAllCountries();

            /*Country lilliput = new Country("Lilliput", "LIL", "Somewhere", 2_000_000);
            int lilliputIndex= countries.FindIndex(x=>x.Population<2_000_000);
            countries.Insert(lilliputIndex, lilliput);
            countries.RemoveAt(lilliputIndex);*/

            var filteredCountries = countries.Where(x => !x.Name.Contains(','));//.Take(20);
            var filteredCountries2 = from country in countries
                                     where !country.Name.Contains(',')
                                     select country;

            foreach (Country country in filteredCountries2)
            {
                Console.WriteLine($"{PopulationFormatter.FormatPopulation(country.Population).PadLeft(15)}: {country.Name}");
            }
            
            Console.ReadLine();
        }
    }
}
