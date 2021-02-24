using BotBinanceBL.Requests;
using BotBinanceBL.Stocks.Interfaces;
using BotBinanceBL.Stocks.Signals;
using Model.Enums;
using Model.Models.Account;
using Model.Models.Market;
using Model.TradingRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotBinanceBL.Stocks
{
    public class Binance : IStock
    {
        private string _url { get; set; } = "https://www.binance.com";
        private string _key { get; set; }
        private string _secretKey { get; set; }

        private BinanceRequest _binanceRequest { get; set; }

        public Binance(string key, string secretKey)
        {
            _key = key;
            _secretKey = secretKey;

            _binanceRequest = new BinanceRequest(new HttpUtilities(_url, key, secretKey));
        }

        public async Task<AccountInfo> GetAccountInformation()
        {
            return await _binanceRequest.GetAccountInfoAsync();
        }
        public async Task<List<Balance>> GetBalance(Symbol asset)
        {
            try
            {
                List<Balance> lst = new List<Balance>();

                var balanceRequest = await _binanceRequest.GetAccountInfoAsync();

                // Узнаем баланс валютсы за которую будем покупать
                lst.AddRange(balanceRequest.Balances
                   .Where(x => x.Asset == asset.QuoteAsset));

                // Узнаем баланс валюты, которую будем продавать
                lst.AddRange(balanceRequest.Balances
                   .Where(x => x.Asset == asset.BaseAsset));

                return lst;
            }
            catch { throw new Exception("Ошибка в методе GetBalance"); };
        }
        public async Task<Trade> GetLastTrade(string symbol)
        {
            try
            {
                IEnumerable<Trade> result = await _binanceRequest.GetTradeList(symbol, 1);

                return result.LastOrDefault();
            }
            catch { throw new Exception($"Ошибка в методе GetLastTrade ({ DateTime.Now })"); }
        }
        public async Task<QueryOCO> GetOcoOrderAsync(OCOOrder order)
        {
            try
            {
                return await _binanceRequest.QueryOCO(order.OrderListId);
            }
            catch { throw new Exception("Ошибка в методе GetOcoOrderAsync"); }
        }
        public async Task<QueryOCO> CanceledOrderAsync(string symbol, OCOOrder order)
        {
            try
            {
                return await _binanceRequest.CancelOCO(symbol, order.ListClientOrderId);
            }
            catch { throw new Exception("Ошибка в методе CanceledOrderAsync"); }
        }
        public async Task<OCOOrder> PostOrderOCOAsync(Signal signal)
        {
            try
            {
                return await _binanceRequest.PostOCOOrder(signal.Symbol, signal.Quantity, signal.Price, signal.StopLoss, signal.StopLimitPrice, signal.Side);
            }
            catch { throw new Exception($"Ошибка в методе PostOrderOCOAsync ({DateTime.Now})"); }
        }
        public async Task<NewOrder> TakeMarketOrder(Signal signal)
        {
            try
            {
                if (signal.Side == OrderSide.BUY)
                {
                    return await _binanceRequest.OrderQuoteMarket(signal.Symbol, signal.Quantity, signal.Side);
                }
                else
                {
                    return await _binanceRequest.OrderQuantityMarket(signal.Symbol, signal.Quantity, signal.Side);
                }
            }
            catch
            {
                throw new Exception($"TakeMarketOrder не сработал на {(signal.Side == OrderSide.BUY ? "покупку" : "продажу")} валюты {signal.Symbol}");
            }
        }
        public async Task<Symbol> GetExchangeInformationAsync(string symbol)
        {
            var result = await _binanceRequest.GetExchangeInformationAsync();

            return result.Symbols.Where(s => symbol.Contains(s.SymbolName)).FirstOrDefault();
        }
        public async Task<IEnumerable<Candlestick>> GetCandlestickAsync(string symbol, TimeInterval timeInterval, int quantity = 50)
        {
            return await _binanceRequest.GetCandleSticks(symbol, timeInterval, limit: quantity);
        }
        public async Task<decimal> GetLastBuyPriceAsync(string symbol)
        {
            Trade lastTrade = await GetLastTrade(symbol);

            return lastTrade.Price;
        }
        public async Task<decimal> GetCurrentPrice(string symbol, TimeInterval timeInterval)
        {
            IEnumerable<Candlestick> candle = await GetCandlestickAsync(symbol, timeInterval, 1);

            return candle.Last().Close;
        }
    
        public async Task TrailingOCO(Signal signal, TimeInterval timeInterval)
        {
            OCOOrder orderOco = await PostOrderOCOAsync(signal);

            await Task.Delay(1500);

            for (ushort i = 0; i < ushort.MaxValue; i++)
            {
                try
                {
                    QueryOCO runningOrder = await GetOcoOrderAsync(orderOco);

                    if (!runningOrder.ListOrderStatus.Equals("ALL_DONE"))
                    {
                        IEnumerable<Candlestick> candlestick = await GetCandlestickAsync(signal.Symbol, timeInterval, 1);

                        if (signal.Price - candlestick.First().Close <= 2.0m)
                        {                        
                            QueryOCO canceledOrder = await _binanceRequest.CancelOCO(signal.Symbol, orderOco.ListClientOrderId);

                            await Task.Delay(2000);

                            ReformSignal(signal);

                            orderOco = await PostOrderOCOAsync(signal);
                        }
                    }
                    await Task.Delay(1000);
                }
                catch (Exception e) { Console.WriteLine(e.Message); continue; }
            }
        }

        private void ReformSignal(Signal signal)
        {
            signal.Price += 0.2m;
            signal.StopLoss += 0.2m;
            signal.StopLimitPrice += 0.2m;
        }    
    }
}
