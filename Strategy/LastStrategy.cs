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
        }

        private List<DataStrategy> ReadCsv(string path)
        {
            
            List<DataStrategy> result = File.ReadAllLines(path).Skip(1)
                .Select(line => line.Split(';'))
                .Select(x => new DataStrategy()
                {
                    LrSlope = Convert.ToDecimal(x[0]),
                    ProfitLong = Convert.ToDecimal(x[1])
                }).ToList();

            return result;
        }
    }

    public class DataStrategy
    {
        #region Periods

        public decimal LrSlope { get; set; }
        public decimal RSI { get; set; }
        public decimal TSIFirstR { get; set; }
        public decimal TSISecondS { get; set; }

        #endregion

        #region Values Long

        public decimal RsiValueLong { get; set; }
        public decimal SlopeValueLong { get; set; }   
        public decimal TSIValueLong { get; set; }

        #endregion

        #region Values Short

        public decimal RsiValueShort { get; set; }
        public decimal SlopeValueShort { get; set; }
        public decimal TSIValueShort { get; set; }

        #endregion

        #region Profit

        public decimal ProfitLong { get; set; }
        public decimal ProfitShort { get; set; }

        #endregion
    }
}
