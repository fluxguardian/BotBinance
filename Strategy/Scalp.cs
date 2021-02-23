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

            _sma = new SimpleMovingAverage(shortPeriod: 90, longPeriod: 400);
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

            for (ushort i = 0; i < ushort.MaxValue; i++)
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
                        await Sell(profitPercent: 1.03m, stopLoss: 1.02m);
                    }

                    await Task.Delay((60 - DateTime.Now.Second + 2) * 1000);
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }

        private async Task Sell(decimal profitPercent, decimal stopLoss)
        {
            decimal lastBuyPrice = await _stock.GetLastBuyPriceAsync(_symbol);

            List<Balance> balance = await _stock.GetBalance(_asset);

            for (ushort i = 0; i < ushort.MaxValue; i++)
            {
                decimal currentPrice = await _stock.GetCurrentPrice(_symbol, _timeInterval);

                if (currentPrice >= lastBuyPrice * profitPercent)
                {
                    await _stock.TrailingOCO(new Signal()
                    {
                        Symbol = _symbol,
                        Side = OrderSide.SELL,
                        Quantity = _normalization.NormalizeSell(balance.Last().Free),
                        Price = currentPrice + 1.0m,
                        StopLoss = currentPrice - 0.6m,
                        StopLimitPrice = currentPrice - 0.5m
                    }, _timeInterval);
                    break;
                }
                else if (currentPrice <= lastBuyPrice / stopLoss)
                {
                    await _stock.TakeMarketOrder(new Signal()
                    {
                        Symbol = _symbol,
                        Side = OrderSide.SELL,
                        Quantity = _normalization.NormalizeSell(balance.Last().Free)
                    });
                    break;
                }
                else
                {
                    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, _sma.LongPeriod + 3);

                    bool sellSignalCross = _sma.SellSignalCross(candles.Select(x => x.Close));
                    if (sellSignalCross)
                    {
                        await _stock.TakeMarketOrder(new Signal()
                        {
                            Symbol = _symbol,
                            Side = OrderSide.SELL,
                            Quantity = _normalization.NormalizeSell(balance.Last().Free)
                        });
                        break;
                    }
                }
                await Task.Delay((60 - DateTime.Now.Second + 2) * 1000);
            }
        }

        private async Task Buy()
        {
            for (ushort i = 0; i < ushort.MaxValue; i++)
            {
                IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, _sma.LongPeriod + 3);
                IEnumerable<decimal> prices = candles.Select(x => x.Close);

                bool buySignalSma = _sma.BuySignalCross(prices);
                //bool buySignalTSI = _tsi.BuySignal(prices);

                if (buySignalSma) //&& buySignalTSI)
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
        }
    }
}
