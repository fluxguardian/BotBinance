using BotBinanceBL.Interfaces;
using BotBinanceBL.Stocks.Interfaces;
using BotBinanceBL.Stocks.Signals;
using Model.Enums;
using Model.Models.Account.Spot;
using Model.Models.Market;
using Model.TradingRules;
using Model.Utils;
using Strategy.TradeSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnicalAnalysis.Oscillators;
using TechnicalAnalysis.Trends;
using TechnicalAnalysis.Volatility;

namespace Strategy
{
    public class Slope : IStrategy
    {
        private IStock _stock { get; set; }
        private string _name { get; set; }
        private string _symbol { get; set; }
        private TimeInterval _timeInterval { get; set; }
        private LinearRegression _linearRegression { get; set; }
        private Normalization _normalization { get; set; }
        private AverageTrueRange _averageTrueRange { get; set; }
        private Symbol _asset { get; set; }

        public Slope(string name, string symbol, TimeInterval timeInterval)
        {
            _name = name;
            _symbol = symbol;
            _timeInterval = timeInterval;

            _linearRegression = new LinearRegression();
            _averageTrueRange = new AverageTrueRange(period: 20);
        }

        public void Trade(IStock stock)
        {
            _stock = stock;

            _asset = _stock.GetExchangeInformationAsync(_symbol).Result;
            _normalization = new Normalization(_asset);

            Logic().Wait();
        }

        private async Task Logic()
        {
            _asset = await _stock.GetExchangeInformationAsync(_symbol);
            _normalization = new Normalization(_asset);

             List<Balance> balance = await _stock.GetBalance(_asset);

            Console.WriteLine($"Баланс пользователя {_name}: {Math.Round(balance.First().Free, _normalization.Round.RoundPrice)} $");

            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    Trade lastTrade = await _stock.GetLastTrade(_symbol);

                    if (lastTrade.IsBuyer == false)
                    {
                        Console.WriteLine($"{_name}: ожидаем вход в рынок {DateTime.Now}");

                        await Buy();

                        await Task.Delay(10000);
                    }
                    else
                    {
                        Console.WriteLine($"{_name}: ожидаем выход из рынка {DateTime.Now}");

                        await Sell();

                        await Task.Delay(10000);

                        #region Баланс после продажи

                        balance = await _stock.GetBalance(_asset);

                        Console.WriteLine($"Баланс пользователя {_name}: {Math.Round(balance.First().Free, _normalization.Round.RoundPrice)} $");

                        #endregion
                    }

                    await Task.Delay((60 - DateTime.Now.Second) * 1000);
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }

        private async Task Sell()
        {
            decimal lastBuyPrice = await _stock.GetLastBuyPriceAsync(_symbol);

            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, quantity: 200);
                    List<decimal> prices = candles.Select(x => x.Close).ToList();

                    LinearRegressionCurve lr = _linearRegression.GetValuesCurve(prices, 195).SkipLast(1).Last();
                    decimal atr = _averageTrueRange.GetATR(candles.ToList()).SkipLast(1).Last();

                    var normalizeSlope = lr.Slope / atr;

                    if (normalizeSlope < 0)
                    {
                        List<Balance> balance = await _stock.GetBalance(_asset);
                        await _stock.TakeMarketOrder(new Signal()
                        {
                            Symbol = _symbol,
                            Side = OrderSide.SELL,
                            Quantity = _normalization.NormalizeSell(balance.Last().Free)
                        });
                        break;
                    }
                    else if(candles.SkipLast(1).Last().Close <= lastBuyPrice / 1.017m)
                    {
                        List<Balance> balance = await _stock.GetBalance(_asset);
                        await _stock.TakeMarketOrder(new Signal()
                        {
                            Symbol = _symbol,
                            Side = OrderSide.SELL,
                            Quantity = _normalization.NormalizeSell(balance.Last().Free)
                        });
                        break;
                    }

                    await Task.Delay(candles.Last().GetTimeSleepMilliseconds() + 3000);
                }
                catch (Exception e) { Console.WriteLine(e.Message); continue; }
            }
        }

        private async Task Buy()
        {
            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, quantity: 200);
                    List<decimal> prices = candles.Select(x => x.Close).ToList();

                    LinearRegressionCurve lr = _linearRegression.GetValuesCurve(prices, 195).SkipLast(1).Last();
                    decimal atr = _averageTrueRange.GetATR(candles.ToList()).SkipLast(1).Last();

                    var normalizeSlope = lr.Slope / atr;

                    if (normalizeSlope > 0)
                    {
                        List<Balance> balance = await _stock.GetBalance(_asset);
                        await _stock.TakeMarketOrder(new Signal()
                        {
                            Symbol = _symbol,
                            Side = OrderSide.BUY,
                            Quantity = _normalization.NormalizeBuy(balance.First().Free)
                        });
                        break;
                    }
                    await Task.Delay(candles.Last().GetTimeSleepMilliseconds() + 3000);
                }
                catch (Exception e) { Console.WriteLine(e.Message); continue; }
            }
        }
    }
}
