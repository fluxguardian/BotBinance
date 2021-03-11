using BotBinanceBL.Interfaces;
using BotBinanceBL.Stocks.Interfaces;
using Model.Enums;
using Model.Models.Account.Margin;
using Model.Models.Market;
using Model.TradingRules;
using Model.Utils;
using Strategy.TradeSettings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnicalAnalysis.Trends;

namespace Strategy
{
    public class ScalpLR : IStrategy
    {
        private IStock _stock { get; set; }
        private string _name { get; set; }
        private string _symbol { get; set; }
        private TimeInterval _timeInterval { get; set; }
        private LinearRegression _linearRegression { get; set; }
        private Normalization _normalization { get; set; }
        private Symbol _asset { get; set; }

        private decimal BorrowedQuoteAsset { get; set; }
        private decimal BorrowedBaseAsset { get; set; }

        public ScalpLR(string symbol, TimeInterval timeInterval, string name)
        {
            _symbol = symbol;
            _timeInterval = timeInterval;
            _name = name;

            _linearRegression = new LinearRegression(shortPeriod: 50, longPeriod: 100);
        }

        public void Trade(IStock stock)
        {
            _stock = stock;

            _asset = _stock.GetExchangeInformationAsync(_symbol).Result;
            _normalization = new Normalization(_asset);

            Logic().Wait();
        }

        public async Task Logic()
        {
            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, _linearRegression.LongPeriod + 2);
                    IEnumerable<decimal> pricesClose = candles.Select(x => x.Close);

                    bool buySignalSMA = _linearRegression.BuySignalCross(pricesClose);
                    bool sellSignalSMA = _linearRegression.SellSignalCross(pricesClose);

                    if (buySignalSMA)
                    {
                        // Входим в лонг позицию
                        await LongEntryPosition();

                        await Task.Delay(5000);

                        // Выходим из лонг позиции
                        await LongExitPosition();
                    }
                    else if (sellSignalSMA)
                    {
                        await ShortEntryPosition();

                        await Task.Delay(5000);

                        await ShortExitPosition();
                    }

                    await Task.Delay(candles.Last().GetTimeSleepMilliseconds() + 2500);
                }
                catch { await Task.Delay(1000); continue; }
            }
        }

        private async Task ShortEntryPosition()
        {
            // Узнаем кол-во и занимаем у биржи базового актива
            BorrowedBaseAsset = _normalization.NormalizeSell(await _stock.GetMaxBorrowAsset(_asset.BaseAsset) * 0.5m);
            AccountBorrow borrowQuoteAsset = await _stock.BorrowQuoteAsset(_asset.BaseAsset, BorrowedBaseAsset);

            await Task.Delay(3000);

            OrderMargin marketSell = await _stock.MarketOrderQuantityMarginAsync(_symbol, BorrowedBaseAsset, OrderSide.SELL);
        }

        private async Task ShortExitPosition()
        {
            IEnumerable<MarginTradeList> lastTrades = await _stock.MarginTradeListsAsync(_symbol, limit: 10);
            decimal lastSellPrice = lastTrades.Where(x => x.IsBuyer == false).LastOrDefault().Price;

            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, 2);

                    // Профит
                    if (candles.FirstOrDefault().Close <= lastSellPrice / 1.025m)
                    {
                        await ExitShort();
                        break;
                    }
                    // Стоп лосс
                    else if (candles.FirstOrDefault().Close >= lastSellPrice * 1.015m)
                    {
                        await ExitShort();
                        break;
                    }

                    await Task.Delay(candles.Last().GetTimeSleepMilliseconds() + 2500);
                }
                catch { await Task.Delay(1000); continue; }
            }
        }

        private async Task LongEntryPosition()
        {
            // Узнаем кол-во и занимаем у биржи котируемого актива
            BorrowedQuoteAsset = _normalization.NormalizeBuy(await _stock.GetMaxBorrowAsset(_asset.QuoteAsset) * 0.5m);
            AccountBorrow borrowQuoteAsset = await _stock.BorrowQuoteAsset(_asset.QuoteAsset, BorrowedQuoteAsset);

            await Task.Delay(3000);

            OrderMargin marketBuy = await _stock.MarketOrderQuoteMarginAsync(_symbol, BorrowedQuoteAsset, OrderSide.BUY);
        }

        private async Task LongExitPosition()
        {
            IEnumerable<MarginTradeList> lastTrades = await _stock.MarginTradeListsAsync(_symbol, limit: 10);
            decimal lastBuyPrice = lastTrades.Where(x => x.IsBuyer == true).LastOrDefault().Price;

            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    IEnumerable<Candlestick> candles = await _stock.GetCandlestickAsync(_symbol, _timeInterval, 2);

                    // Профит
                    if (candles.First().Close >= lastBuyPrice * 1.03m)
                    {
                        await ExitLong();
                        break;
                    }
                    // Стоп лосс
                    else if (candles.First().Close <= lastBuyPrice / 1.015m)
                    {
                        await ExitLong();
                        break;
                    }

                    await Task.Delay(candles.Last().GetTimeSleepMilliseconds() + 2500);
                }
                catch { await Task.Delay(1000); continue; }
            }
        }

        private async Task ExitShort()
        {
            // Продаем занимаемое кол-во валюты
            OrderMargin marketSell = await _stock.MarketOrderQuantityMarginAsync(_symbol, BorrowedBaseAsset, OrderSide.BUY);
            await Task.Delay(3000);

            // Возмещаем бирже
            AccountRepay repaid = await _stock.RepayQuoteAsset(_asset.BaseAsset);
        }

        private async Task ExitLong()
        {
            // Продаем занимаемое кол-во валюты
            OrderMargin marketSell = await _stock.MarketOrderQuoteMarginAsync(_symbol, BorrowedQuoteAsset, OrderSide.SELL);
            await Task.Delay(3000);

            // Возмещаем бирже
            AccountRepay repaid = await _stock.RepayQuoteAsset(_asset.QuoteAsset);
        }
    }
}
