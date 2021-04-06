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
using System.Threading;
using System.Threading.Tasks;
using TechnicalAnalysis.Oscillators;
using TechnicalAnalysis.Trends;
using TechnicalAnalysis.Volatility;

namespace Strategy
{
    public class Scalp : IStrategy
    {
        private IStock _stock { get; set; }
        private string _name { get; set; }
        private string _symbol { get; set; }
        private TimeInterval _timeInterval { get; set; }
        private LinearRegression _linearRegression { get; set; }
        private TrueStrengthIndex _tsi { get; set; }
        private Normalization _normalization { get; set; }
        private AverageTrueRange _averageTrueRange { get; set; }
        private Symbol _asset { get; set; }

        public Scalp(string symbol, TimeInterval timeInterval, string name)
        {
            _symbol = symbol;
            _timeInterval = timeInterval;
            _name = name;

            _linearRegression = new LinearRegression(shortPeriod: 300, longPeriod: 590);
            _tsi = new TrueStrengthIndex(30, 12, 18);
            _averageTrueRange = new AverageTrueRange(period: 10);
        }

        public void Trade(IStock stock)
        {
            _stock = stock;

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

                        await Task.Delay(15000);

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
            List<Balance> balances = await _stock.GetBalance(_asset);

            Signal signal = await FormingSignal(lastBuyPrice, balances);
            OCOOrder ocoOrder = await _stock.PostOrderOCOAsync(signal);

            await Task.Delay(5000);

            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    QueryOCO runningOrder = await _stock.GetOcoOrderAsync(ocoOrder);

                    if (runningOrder.ListOrderStatus.Equals("EXECUTING"))
                    {
                        QueryOCO canceledOrder = await _stock.CanceledOrderAsync(_symbol, ocoOrder);

                        await Task.Delay(3000);

                        signal = await FormingSignal(lastBuyPrice, balances);
                        ocoOrder = await _stock.PostOrderOCOAsync(signal);
                    }
                    else if (runningOrder.ListOrderStatus.Equals("ALL_DONE")) { break; }

                    IEnumerable<Candlestick> candle = await _stock.GetCandlestickAsync(_symbol, _timeInterval, quantity: 1);
                    await Task.Delay(candle.Last().GetTimeSleepMilliseconds() + 3000);
                }
                catch { Console.WriteLine("Ошибка в методе Sell"); await Task.Delay(1000); }
            }
        }
        private async Task Buy()
        {
            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, quantity: _linearRegression.LongPeriod + 3);
                    List<decimal> prices = candles.Select(x => x.Close).ToList();

                    bool buySignalLR = _linearRegression.BuySignal(prices);
                    TSIValues tSI = _tsi.GetTSI(prices);

                    if (buySignalLR && tSI.TSI.Last() > 0 && tSI.SignalLine.Last() > 0.1m)
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

        private async Task<int> GetHoldBars()
        {
            Trade lastTrade = await _stock.GetLastTrade(_symbol);
            DateTime lastBuyTime = DateTimeOffset.FromUnixTimeMilliseconds(lastTrade.Time).LocalDateTime;

            var candle = await _stock.GetCandlestickAsync(_symbol, _timeInterval, quantity: 1);
            DateTime candleCloseTime = DateTimeOffset.FromUnixTimeMilliseconds(candle.FirstOrDefault().CloseTime).LocalDateTime;

            double quantityMinutes = (candleCloseTime - lastBuyTime).TotalMinutes;

            return (int)Math.Round(quantityMinutes / 3.0, 1);
        }
        private async Task<Signal> FormingSignal(decimal lastBuyPrice, List<Balance> balances)
        {
            decimal kStop = 4.0m;
            decimal kHoldBars = 0.24m;

            IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, quantity: 150);

            List<decimal> atr = _averageTrueRange.GetATR(candles.ToList());
            decimal holdBars = await GetHoldBars();

            decimal priceStop = ((lastBuyPrice - atr.SkipLast(1).Last() * kStop)) + (holdBars * holdBars * kHoldBars);

            return new Signal()
            {
                Symbol = _symbol,
                Side = OrderSide.SELL,
                Quantity = _normalization.NormalizeSell(balances.Last().Free),
                Price = Math.Round(candles.Last().Close * 1.05m, _normalization.Round.RoundPrice),
                StopLoss = Math.Round(priceStop * 1.0015m, _normalization.Round.RoundPrice),
                StopLimitPrice = Math.Round(priceStop, _normalization.Round.RoundPrice)
            };
        }

        Task IStrategy.Logic()
        {
            throw new NotImplementedException();
        }
    }
}
