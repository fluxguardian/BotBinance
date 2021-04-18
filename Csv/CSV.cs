using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Csv
{
    public static class CSV<T> where T : class
    {
        public static List<T> ReadCsv(string path)
        {
            CsvConfiguration configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                
            };

            using (StreamReader streamReader = new StreamReader(path))
            {
                using (CsvReader csvReader = new CsvReader(streamReader, configuration))
                {
                    return csvReader.GetRecords<T>().ToList();
                }
            }
        }
    }
}
