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
    public class RsiSlope : IStrategy
    {
        private IStock _stock { get; set; }
        private string _name { get; set; }
        private string _symbol { get; set; }
        private TimeInterval _timeInterval { get; set; }
        private LinearRegression _linearRegression { get; set; }
        private Normalization _normalization { get; set; }
        private AverageTrueRange _averageTrueRange { get; set; }
        private RelativeStrengthIndex _rsi { get; set; }
        private Symbol _asset { get; set; }

        public RsiSlope(string name, string symbol, TimeInterval timeInterval)
        {
            _name = name;
            _symbol = symbol;
            _timeInterval = timeInterval;

            _linearRegression = new LinearRegression();
            _averageTrueRange = new AverageTrueRange(period: 50);
            _rsi = new RelativeStrengthIndex(period: 4);
        }

        public void Trade(IStock stock)
        {
            _stock = stock;

            Logic().Wait();
        }

        public async Task Logic()
        {
            await Init();

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
                        await Task.Delay(5000);

                        await WriteConsoleLastTrade();
                    }
                    else
                    {
                        Console.WriteLine($"{_name}: ожидаем выход из рынка {DateTime.Now.ToLocalTime()}");

                        await Sell();
                        await Task.Delay(5000);

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

        public async Task Init()
        {
            _asset = await _stock.GetExchangeInformationAsync(_symbol);
            _normalization = new Normalization(_asset);
        }

        private async Task Sell()
        {
            decimal lastBuyPrice = await _stock.GetLastBuyPriceAsync(_symbol);
            List<Balance> balances = await _stock.GetBalance(_asset);

            IEnumerable<Order> opensLimitOrders = await _stock.GetCurrentOpenOrders(_symbol);

            if (!opensLimitOrders.Any())
            {
                await _stock.TakeOrderStopLossLimit(new Signal()
                {
                    Symbol = _symbol,
                    Side = OrderSide.SELL,
                    Quantity = _normalization.NormalizeSell(balances.Last().Free),
                    StopLoss = Math.Round(lastBuyPrice / 1.0015m, _normalization.Round.RoundPrice),
                    StopLimitPrice = Math.Round(lastBuyPrice / 1.002m, _normalization.Round.RoundPrice),
                });
            }

            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, quantity: _rsi.Period * 8);
                    List<decimal> prices = candles.Select(x => x.Close).ToList();

                    decimal rsi = _rsi.GetRSI(prices).SkipLast(1).Last();

                    if (rsi > 65 && prices.SkipLast(1).Last() >= lastBuyPrice * 1.003m)
                    {
                        opensLimitOrders = await _stock.GetCurrentOpenOrders(_symbol);
                        if (opensLimitOrders.Any())
                        {
                            CanceledOrder canceledOrder = await _stock.CancelLimitOrder(opensLimitOrders.First());

                            await Task.Delay(3000);

                            await MarketSell();

                            break;
                        }
                        else { break; }
                    }

                    await Task.Delay(candles.Last().GetTimeSleepMilliseconds() + 3000);
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }

        public async Task Buy()
        {
            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    decimal normalizeSlope = await GetNormalizeLrSlope(6);

                    if (normalizeSlope < -60)
                    {
                        await MarketBuy();
                        break;
                    }

                    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, quantity: 1);
                    await Task.Delay(candles.Last().GetTimeSleepMilliseconds() + 3000);
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }

        private async Task<decimal> GetNormalizeLrSlope(int periodLr)
        {
            IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, quantity: _averageTrueRange.Period * 6);
            List<decimal> prices = candles.Select(x => x.Close).ToList();

            List<decimal> rsi = _rsi.GetRSI(prices);

            decimal slopeLr = _linearRegression.GetValuesCurve(rsi, periodLr).SkipLast(1).Last().Slope;
            decimal atr = _averageTrueRange.GetATR(candles.ToList()).SkipLast(1).Last();

            return slopeLr / atr;
        }

        private async Task MarketSell()
        {
            List<Balance> balance = await _stock.GetBalance(_asset);
            await _stock.TakeMarketOrder(new Signal()
            {
                Symbol = _symbol,
                Side = OrderSide.SELL,
                Quantity = _normalization.NormalizeSell(balance.Last().Free)
            });
        }
        private async Task MarketBuy()
        {
            List<Balance> balance = await _stock.GetBalance(_asset);
            await _stock.TakeMarketOrder(new Signal()
            {
                Symbol = _symbol,
                Side = OrderSide.BUY,
                Quantity = _normalization.NormalizeBuy(balance.First().Free)
            });
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
