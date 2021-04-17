using BotBinanceBL.Interfaces;
using BotBinanceBL.Stocks.Interfaces;
using Model.Enums;
using Model.Models.Market;
using Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechnicalAnalysis.Interfaces;

namespace Strategy
{
    public class IndicatorSignal : IStrategy
    {
        private List<IIndicator> indicators { get; set; }
        private IStock _stock { get; set; }
        private List<string> _symbols { get; set; }
        private TimeInterval _timeInterval { get; set; }

        public IndicatorSignal(List<IIndicator> indicators, List<string> symbols, TimeInterval timeInterval)
        {
            this.indicators = indicators;

            _symbols = symbols;
            _timeInterval = timeInterval;
        }

        public void Trade(IStock stock)
        {
            _stock = stock;

            Logic().Wait();
        }

        public async Task Logic()
        {
            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    //await Task.WhenAll(_symbols.Select(async x =>
                    //{
                    //    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(x, _timeInterval, quantity: 150);

                    //    bool boolsBuy = !indicators.Select(a => a.IsBuy(candles.SkipLast(1).ToList())).Any(x => x == false);
                    //    bool boolsSell = !indicators.Select(a => a.IsSell(candles.SkipLast(1).ToList())).Any(x => x == false);

                    //    if (boolsBuy)
                    //    {
                    //        Console.WriteLine($"Buy Signal: symbol -- {x}, time: {DateTime.Now.ToLocalTime()}");
                    //    }
                    //    else if (boolsSell)
                    //    {
                    //        Console.WriteLine($"Sell Signal: symbol -- {x}, time: {DateTime.Now.ToLocalTime()}");
                    //    }

                    //})).ContinueWith(x => WaitTime().Wait());


                    List<bool> lstBuys = new List<bool>();
                    List<bool> lstSell = new List<bool>();

                    foreach (string symbol in _symbols)
                    {
                        IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(symbol, _timeInterval, quantity: 150);

                        foreach (IIndicator indicator in indicators)
                        {

                            lstBuys.Add(indicator.IsBuy(candles.SkipLast(1).ToList()));
                            lstSell.Add(indicator.IsSell(candles.SkipLast(1).ToList()));
                        }

                        if (!lstBuys.Any(x => x == false))
                        {
                            Console.WriteLine($"Buy Signal: symbol -- {symbol}, time: {DateTime.Now.ToLocalTime()}");
                        }

                        if (!lstSell.Any(x => x == false))
                        {
                            Console.WriteLine($"Sell Signal: symbol -- {symbol}, time: {DateTime.Now.ToLocalTime()}");
                        }

                        lstBuys = new List<bool>();
                        lstSell = new List<bool>();
                    }

                }
                catch (Exception e) { Console.WriteLine(e.Message); }

            }
        }

        public async Task WaitTime()
        {
            var candlesLast = await _stock.GetCandlestickAsync(_symbols.Last(), _timeInterval, quantity: 1);
            await Task.Delay(candlesLast.Last().GetTimeSleepMilliseconds() + 2500);
        }
    }
}
