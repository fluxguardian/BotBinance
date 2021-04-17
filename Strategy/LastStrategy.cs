using BotBinanceBL.Interfaces;
using BotBinanceBL.Stocks.Interfaces;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TechnicalAnalysis.Interfaces;
using System.Linq;
using System.Globalization;
using Csv;

namespace Strategy
{
    public class LastStrategy : IStrategy
    {
        private List<IIndicator> indicators { get; set; }
        private TimeInterval _timeInterval { get; set; }
        private IStock _stock { get; set; }

        private static string appPapth => Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

        public LastStrategy(List<IIndicator> indicators, TimeInterval timeInterval)
        {
            this.indicators = indicators;
            _timeInterval = timeInterval;
        }
        public void Trade(IStock stock)
        {
            _stock = stock;

            Logic().Wait();
        }

        public async Task Logic()
        {
            var res = ReadCsv(appPapth + @"\Data\DataLastStrategy\DataOfIndicators.csv");

            Console.WriteLine(res.First().LrSlope);
            Console.WriteLine(res.First().ProfitLong.ToString(CultureInfo.InvariantCulture));
        }

        private List<DataStrategy> ReadCsv(string path)
        {

            //List<DataStrategy> result = File.ReadAllLines(path).Skip(1)
            //    .Select(line => line.Split(';'))
            //    .Select(x => new DataStrategy()
            //    {
            //        LrSlope = Convert.ToDecimal(x[0]),
            //        ProfitLong = Convert.ToDecimal(x[1].Replace('.', ','))
            //    }).ToList();

            var result = CSV.ReadCsv(path);

            return result;
        }
    } 
}
