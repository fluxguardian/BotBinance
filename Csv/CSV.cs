using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Csv
{
    public static class CSV
    {
        public static List<DataStrategy> ReadCsv(string path)
        {
            CsvConfiguration configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                
            };

            using (StreamReader streamReader = new StreamReader(path))
            {
                using (CsvReader csvReader = new CsvReader(streamReader, configuration))
                {
                    return csvReader.GetRecords<DataStrategy>().ToList();
                }
            }
        }
    }
}
