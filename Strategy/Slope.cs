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

            Logic().Wait();
        }

        public async Task Logic()
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
                        Console.WriteLine($"{_name}: ожидаем вход в рынок {DateTime.Now.ToLocalTime()}");

                        await Buy();
                        await Task.Delay(10000);

                        await WriteConsoleLastTrade();
                    }
                    else
                    {
                        Console.WriteLine($"{_name}: ожидаем выход из рынка {DateTime.Now.ToLocalTime()}");

                        await Sell();
                        await Task.Delay(10000);

                        #region Баланс после продажи

                        balance = await _stock.GetBalance(_asset);

                        Console.WriteLine($"Баланс пользователя {_name}: {Math.Round(balance.First().Free, _normalization.Round.RoundPrice)} $");

                        await WriteConsoleLastTrade();

                        #endregion
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }

        public async Task Sell()
        {
            decimal lastBuyPrice = await _stock.GetLastBuyPriceAsync(_symbol);

            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, quantity: 120);
                    List<decimal> prices = candles.Select(x => x.Close).ToList();

                    LinearRegressionCurve lr = _linearRegression.GetValuesCurve(prices, 21).SkipLast(1).Last();
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
                    else if(candles.SkipLast(1).Last().Close <= lastBuyPrice / 1.01m)
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
        public async Task Buy()
        {
            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, quantity: 120);
                    List<decimal> prices = candles.Select(x => x.Close).ToList();

                    LinearRegressionCurve lr = _linearRegression.GetValuesCurve(prices, 21).SkipLast(1).Last();
                    decimal atr = _averageTrueRange.GetATR(candles.ToList()).SkipLast(1).Last();

                    decimal normalizeSlope = lr.Slope / atr;

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

        private async Task WriteConsoleLastTrade()
        {
            Trade lastTrade = await _stock.GetLastTrade(_symbol);

            Console.WriteLine("-----------------------------------------");

            Console.WriteLine($"Пользователь: {_name}");

            if (lastTrade.IsBuyer)
            {
                Console.WriteLine($"Цена покупки: {Math.Round(lastTrade.Price, _normalization.Round.RoundPrice)}");
            }
            else { Console.WriteLine($"Цена продажи: {Math.Round(lastTrade.Price, _normalization.Round.RoundPrice)}"); }

            Console.WriteLine($"Комиссия: {lastTrade.Commission} {lastTrade.CommissionAsset}");

            Console.WriteLine("-----------------------------------------");
        }
    }
}
