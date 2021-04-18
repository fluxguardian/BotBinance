using BotBinanceBL.Interfaces;
using BotBinanceBL.Stocks.Interfaces;
using Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Model.Models.Market;
using TechnicalAnalysis.Oscillators;
using Strategy.Data.DataLastStrategy;
using TechnicalAnalysis.Trends;
using System;
using Model.Utils;

namespace Strategy
{
    public class LastStrategy : IStrategy
    {
        private TimeInterval _timeInterval { get; set; }
        private IStock _stock { get; set; }
        private List<DataStrategy> DataStrategies { get; set; }
        private TrueStrengthIndex trueStrengthIndex { get; set; }
        private RelativeStrengthIndex relativeStrengthIndex { get; set; }
        private LinearRegression linearRegression { get; set; }

        public LastStrategy(List<DataStrategy> dataStrategies, TimeInterval timeInterval)
        {
            _timeInterval = timeInterval;

            trueStrengthIndex = new TrueStrengthIndex();
            relativeStrengthIndex = new RelativeStrengthIndex();
            linearRegression = new LinearRegression();

            DataStrategies = dataStrategies;
        }
        public void Trade(IStock stock)
        {
            _stock = stock;

            Logic().Wait();
        }

        public async Task Logic()
        {
            List<string> symbols = DataStrategies.Select(x => x.Symbol).ToList();

            for (uint i = 0; i < uint.MaxValue; i++)
            {
                Dictionary<string, string> result = await WaitSignal(symbols);

                if (result.Any())
                {
                    if (result.First().Value == "Long")
                    {

                    }
                    else if (result.First().Value == "Short") { }
                }

                await WaitNextCandle();
            }

            Console.WriteLine();
        }

        private async Task<Dictionary<string, string>> WaitSignal(List<string> symbols)
        {
            var tasks = new Task<Dictionary<string, string>>[symbols.Count];
            Task<Dictionary<string, string>[]> allTasks = null;

            int i = 0;

            try
            {
                foreach (string symbol in symbols)
                {
                    tasks[i] = Task.Run(() => CheckBuyOrSell(symbol));
                    i++;
                }

                allTasks = Task.WhenAll(tasks);
                await allTasks;

                return allTasks.Result.Where(x => x != null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Исключение: " + ex.Message);
                Console.WriteLine("IsFaulted: " + allTasks.IsFaulted);
                foreach (var inx in allTasks.Exception.InnerExceptions)
                {
                    Console.WriteLine("Внутреннее исключение: " + inx.Message);
                }

                return null;
            }
        }

        private Dictionary<string, string> CheckBuyOrSell(string symbol)
        {
            List<Candlestick> candles = _stock.GetCandlestickAsync(symbol, _timeInterval, quantity: 150).Result.SkipLast(1).ToList();

            DataStrategy parametres = DataStrategies.Where(x => x.Symbol.Equals(symbol)).FirstOrDefault();

            List<bool> longs = new List<bool>()
            {
                trueStrengthIndex.IsBuy(candles, parametres.TSIValueLong / 100.0m, parametres.TSIFirstR, parametres.TSISecondS),
                relativeStrengthIndex.IsBuy(candles, parametres.RsiValueLong, parametres.RSI),
                linearRegression.IsBuy(candles, parametres.SlopeValueLong, parametres.LrSlope)
            };

            List<bool> shorts = new List<bool>()
            {
                trueStrengthIndex.IsSell(candles, parametres.TSIValueLong / 100.0m, parametres.TSIFirstR, parametres.TSISecondS),
                relativeStrengthIndex.IsSell(candles, parametres.RsiValueLong, parametres.RSI),
                linearRegression.IsSell(candles, parametres.SlopeValueLong, parametres.LrSlope)
            };

            if (!longs.Any(x => x == false))
            {
                return new Dictionary<string, string>() { { symbol, "Long" } };
            }
            else if(!shorts.Any(x => x == false))
            {
                return new Dictionary<string, string>() { { symbol, "Short" } };
            }

            return null;
        }

        private async Task WaitNextCandle()
        {
            var candlesLast = await _stock.GetCandlestickAsync("BTCUSDT", _timeInterval, quantity: 1);
            await Task.Delay(candlesLast.Last().GetTimeSleepMilliseconds() + 2000);
        }
    } 
}
