using BotBinanceBL.Interfaces;
using BotBinanceBL.Stocks.Interfaces;
using Model.Enums;
using Model.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalAnalysis.Oscillators;

namespace Strategy
{
    public class RsiSignal : IStrategy
    {
        public TimeInterval TimeInterval { get; set; }
        public List<string> Symbols { get; set; }
        public int Period { get; set; }
        private IStock Stock { get; set; }

        public RsiSignal(List<string> symbols, int period, TimeInterval timeInterval)
        {
            Period = period;
            TimeInterval = timeInterval;
            Symbols = symbols;
        }

        public void Trade(IStock stock)
        {
            Stock = stock;
            
            //GetSignal(Symbols.First()).Wait();

            var result = Symbols.Select(x => GetSignal(x)).ToList();

            Task.WhenAll(result);  
        }

        private async Task GetSignal(string symbol)
        {
            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    IEnumerable<Candlestick> candles = await Stock.GetCandlestickAsync(symbol, TimeInterval, quantity: 100);

                    List<decimal> valuesRSI = RelativeStrengthIndex.GetRSI(candles.Select(x => x.Close).ToList(), Period);

                    if (valuesRSI.SkipLast(1).Last() < 30.0m)
                    {
                        Console.WriteLine($"Symbol: {symbol}, RSI less than 30, {DateTime.Now}");
                    }
                    else if (valuesRSI.SkipLast(1).Last() > 65.0m)
                    {
                        Console.WriteLine($"Symbol: {symbol}, RSI more than 65, {DateTime.Now}");
                    }

                    var closeTime = DateTimeOffset.FromUnixTimeMilliseconds(candles.Last().CloseTime).LocalDateTime;

                    var timeSleep = (int)(closeTime - DateTime.Now).TotalMilliseconds;

                    await Task.Delay(timeSleep);
                }
                catch { Console.WriteLine($"Ошибка: {symbol}"); await Task.Delay(1500); continue; }
            }
        }
    }
}
