using BotBinanceBL.Interfaces;
using BotBinanceBL.Stocks.Interfaces;
using BotBinanceBL.Stocks.Signals;
using Model.Enums;
using Model.Models.Account;
using Model.Models.Market;
using Model.TradingRules;
using Strategy.TradeSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnicalAnalysis.Oscillators;
using TechnicalAnalysis.Volatility;
using Trends.TechnicalAnalysis;

namespace Strategy
{
    public class Scalp : IStrategy
    {
        private IStock _stock { get; set; }
        private string _name { get; set; }
        private string _symbol { get; set; }
        private TimeInterval _timeInterval { get; set; }
        private SimpleMovingAverage _sma { get; set; }
        private TrueStrengthIndex _tsi { get; set; }
        private Normalization _normalization { get; set; }
        private AverageTrueRange _averageTrueRange { get; set; }
        private Symbol _asset { get; set; }

        public Scalp(string symbol, TimeInterval timeInterval, string name)
        {
            _symbol = symbol;
            _timeInterval = timeInterval;
            _name = name;

            _sma = new SimpleMovingAverage(shortPeriod: 90, longPeriod: 200);
            _tsi = new TrueStrengthIndex(25, 13, 13);
            _averageTrueRange = new AverageTrueRange(period: 14);
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
                        Console.WriteLine("Ожидаем вход в рынок");

                        await Buy();
                    }
                    else
                    {
                        Console.WriteLine("Ожидаем выход из рынка");

                        await Sell(profitPercent: 1.035m, stopLoss: 1.02m, stopLimitPrice: 1.022m);

                        await Task.Delay(2500);

                        #region Баланс после продажи

                        balance = await _stock.GetBalance(_asset);

                        Console.WriteLine($"Баланс пользователя {_name}: {Math.Round(balance.First().Free, _normalization.Round.RoundPrice)} $");

                        #endregion

                        await WaitCross();
                    }

                    await Task.Delay((60 - DateTime.Now.Second + 2) * 1000);
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }

        private async Task WaitCross()
        {
            Console.WriteLine("Ожидаем пересечение скользящих (сверху-вниз), чтобы след покупка была только после пересечении (снизу-верх)");

            for (uint i = 0; i < uint.MaxValue; i++)
            {
                IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, _sma.LongPeriod + 3);

                bool signalSell = _sma.SellSignal(candles.Select(x => x.Close));
                if (signalSell)
                {
                    break;
                }

                await Task.Delay((60 - DateTime.Now.Second + 2) * 1000);
            }
        }
        private async Task Sell(decimal profitPercent, decimal stopLoss, decimal stopLimitPrice)
        {
            decimal lastBuyPrice = await _stock.GetLastBuyPriceAsync(_symbol);

            List<Balance> balance = await _stock.GetBalance(_asset);

            var orderOco = await _stock.PostOrderOCOAsync(new Signal()
            {
                Symbol = _symbol,
                Side = OrderSide.SELL,
                Quantity = _normalization.NormalizeSell(balance.Last().Free),
                Price = Math.Round(lastBuyPrice * profitPercent, _normalization.Round.RoundPrice),
                StopLoss = Math.Round(lastBuyPrice / stopLoss, _normalization.Round.RoundPrice),
                StopLimitPrice = Math.Round(lastBuyPrice / stopLimitPrice, _normalization.Round.RoundPrice)
            });

            await Task.Delay(3000);

            for (uint i = 0; i < uint.MaxValue; i++)
            {
                QueryOCO runningOrder = await _stock.GetOcoOrderAsync(orderOco);

                if (runningOrder.ListOrderStatus.Equals("ALL_DONE"))
                {
                    break;
                }
                await Task.Delay(10000);
            }

                //if (currentPrice >= lastBuyPrice * profitPercent)
                //{
                //    //await _stock.TrailingOCO(new Signal()
                //    //{
                //    //    Symbol = _symbol,
                //    //    Side = OrderSide.SELL,
                //    //    Quantity = _normalization.NormalizeSell(balance.Last().Free),
                //    //    Price = Math.Round(currentPrice + 1.2m, _normalization.Round.RoundPrice),
                //    //    StopLoss = Math.Round(currentPrice - 0.6m, _normalization.Round.RoundPrice),
                //    //    StopLimitPrice = Math.Round(currentPrice - 0.65m, _normalization.Round.RoundPrice)
                //    //}, _timeInterval);

                //    await _stock.TakeMarketOrder(new Signal()
                //    {
                //        Symbol = _symbol,
                //        Side = OrderSide.SELL,
                //        Quantity = _normalization.NormalizeSell(balance.Last().Free)
                //    });

                //    break;
                //}
                //else if (currentPrice <= lastBuyPrice / stopLoss)
                //{
                //    await _stock.TakeMarketOrder(new Signal()
                //    {
                //        Symbol = _symbol,
                //        Side = OrderSide.SELL,
                //        Quantity = _normalization.NormalizeSell(balance.Last().Free)
                //    });
                //    break;
                //}
                //else
                //{
                //    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, _sma.LongPeriod + 3);

                //    bool sellSignalCross = _sma.SellSignalCross(candles.Select(x => x.Close));
                //    if (sellSignalCross)
                //    {
                //        await _stock.TakeMarketOrder(new Signal()
                //        {
                //            Symbol = _symbol,
                //            Side = OrderSide.SELL,
                //            Quantity = _normalization.NormalizeSell(balance.Last().Free)
                //        });
                //        break;
                //    }
                //}
                
            //}
        }
        private async Task Buy()
        {
            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, _sma.LongPeriod + 3);
                    IEnumerable<decimal> prices = candles.Select(x => x.Close);

                    bool buySignalSma = _sma.BuySignal(prices);
                    bool buySignalTSI = _tsi.BuySignal(prices);

                    if (buySignalSma && buySignalTSI)
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
                    await Task.Delay((60 - DateTime.Now.Second + 2) * 1000);
                }
                catch (Exception e) { Console.WriteLine(e.Message); continue; }
            }
        }
    }
}
