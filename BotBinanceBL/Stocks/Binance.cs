using BotBinanceBL.Requests;
using BotBinanceBL.Stocks.Interfaces;
using BotBinanceBL.Stocks.Signals;
using Model.Enums;
using Model.Models.Account.Margin;
using Model.Models.Account.Spot;
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
        private string _url { get; set; } = "https://api.binance.com";
        private BinanceRequest _binanceRequest { get; set; }

        public Binance(string key, string secretKey)
        {
            _binanceRequest = new BinanceRequest(new HttpUtilities(_url, key, secretKey));
        }

        #region Margin Trades

        /// <summary>
        /// Получаем максимальное кол-во занимаемой валюты
        /// </summary>
        /// <param name="asset">Валюта (LINK, USDT, BTC и т.д)</param>
        /// <returns>Максимальное кол-во занимаемой валюты</returns>
        public async Task<decimal> GetMaxBorrowAsset(string asset)
        {
            // Узнаем максимальное количество валюты, которое можем занять у биржы при нашем счете
            MaxBorrow borrow = await _binanceRequest.MaxBorrow(asset);

            return borrow.Amount;
        }

        /// <summary>
        /// Заем базового актива у биржы
        /// </summary>
        /// <param name="asset">Базовая валюта. Например, валютная пара LINKUSDT, в которой LINK является базовой)</param>
        /// <returns></returns>
        public async Task<AccountBorrow> BorrowBaseAsset(string asset, decimal amountBorrow)
        {
            return await _binanceRequest.Borrow(asset, amountBorrow);
        }

        /// <summary>
        /// Заем котируемого актива у биржы
        /// </summary>
        /// <param name="asset">Основная валюта. Например, валютная пара LINKUSDT, в которой USDT является котируемой)</param>
        /// <returns></returns>
        public async Task<AccountBorrow> BorrowQuoteAsset(string asset, decimal amountBorrow)
        {
            return await _binanceRequest.Borrow(asset, amountBorrow);
        }

        /// <summary>
        /// Узнаем кол-во валюты, которое необходимо возместить бирже
        /// </summary>
        /// <param name="asset">Возмещаемая валюта</param>
        /// <returns>Количество</returns>
        private async Task<decimal> GetAmountRepay(string asset)
        {
            QueryMarginAccountDetails accountDetails = await _binanceRequest.MarginAccountDetails();

            IEnumerable<UserAsset> repay = accountDetails.UserAssets.Where(x => x.Asset.Equals(asset));

            return repay.FirstOrDefault().Borrowed;
        }
        public async Task<AccountRepay> RepayQuoteAsset(string asset)
        {
            decimal amountRepay = await GetAmountRepay(asset);

            return await _binanceRequest.Repay(asset, amountRepay);
        }
        public async Task<AccountRepay> RepayBaseAsset(string asset)
        {
            decimal amountRepay = await GetAmountRepay(asset);

            return await _binanceRequest.Repay(asset, amountRepay);
        }
    

        #endregion

        #region Margin methods

        public async Task<AccountTransfer> TransferAsync(string asset, decimal amount, TransferType transferType)
        {
            try
            {
                return await _binanceRequest.Transfer(asset, amount, transferType);
            }
            catch { throw new Exception($"Ошибка в методе TransferAsync ({DateTime.Now})"); }
        }
        public async Task<OrderMargin> MarketOrderQuantityMarginAsync(string symbol, decimal baseAssetQuantity, OrderSide side, string isIsolated = "FALSE")
        {
            try
            {
                return await _binanceRequest.MarketOrderQuantityMargin(symbol, baseAssetQuantity, side);
            }
            catch { throw new Exception($"Ошибка в методе MarketOrderMarginAsync {DateTime.Now}"); }
        }
        public async Task<OrderMargin> MarketOrderQuoteMarginAsync(string symbol, decimal quoteOrderQty, OrderSide side, string isIsolated = "FALSE")
        {
            try
            {
                return await _binanceRequest.MarketOrderQuoteMargin(symbol, quoteOrderQty, side);
            }
            catch { throw new Exception($"Ошибка в методе MarketOrderQuoteMargin {DateTime.Now}"); }
        }
        public async Task<MaxTransferOutAmount> MaxTransferOutAmountAsync(string asset, string isIsolated = "FALSE")
        {
            try
            {
                return await _binanceRequest.MaxTransferOutAmount(asset, isIsolated);
            }
            catch { throw new Exception($"Ошибка в методе MaxTransferOutAmountAsync {DateTime.Now}"); }
        }
        public async Task<OrderMargin> OrderMarginStopLossAsync(string symbol, decimal quantity, decimal price, decimal stopPrice, 
            OrderSide orderSide, OrderType orderType, TimeInForce timeInForce = TimeInForce.GTC)
        {
            try
            {
                return await _binanceRequest.OrderMarginStopLoss(symbol, quantity, price, stopPrice, orderSide, orderType, timeInForce);
            }
            catch { throw new Exception($"Ошибка в методе OrderMarginStopLossAsync {DateTime.Now}"); }
        }
        public async Task<CanceledOrderMargin> CancelOrderMarginAsync(OrderMargin orderMargin)
        {
            try
            {
                return await _binanceRequest.CancelOrderMargin(orderMargin.Symbol, orderMargin.ClientOrderId);
            }
            catch { throw new Exception($"Ошибка в методе CancelOrderMarginAsync {DateTime.Now}"); }
        }
        public async Task<QueryMarginOrder> QueryMarginOrderAsync(OrderMargin orderMargin)
        {
            try
            {
                return await _binanceRequest.QueryMarginOrder(orderMargin.Symbol, orderMargin.ClientOrderId, orderMargin.IsIsolated.ToString().ToUpper());
            }
            catch { throw new Exception($"Ошибка в методе QueryMarginOrderAsync {DateTime.Now}"); }
        }
        public async Task<QueryRepay> QueryRepayRecordAsync(string asset, DateTime startTime, DateTime? endTime = null, long size = 10)
        {
            try
            {
                return await _binanceRequest.QueryRepayRecord(asset, startTime, endTime, size);
            }
            catch { throw new Exception($"Ошибка в методе QueryRepayAsync {DateTime.Now}"); }
        }
        public async Task<QueryMarginAccountDetails> MarginAccountDetailsAsync()
        {
            try
            {
                return await _binanceRequest.MarginAccountDetails();
            }
            catch { throw new Exception($"Ошибка в методе MarginAccountDetailsAsync {DateTime.Now}"); }
        }
        public async Task<IEnumerable<MarginTradeList>> MarginTradeListsAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? fromId = null, string isIsolated = "FALSE", int limit = 500)
        {
            try
            {
                return await _binanceRequest.MarginTradeLists(symbol, startTime, endTime, fromId, isIsolated, limit);
            }
            catch { throw new Exception($"Ошибка в методе MarginTradeListsAsync {DateTime.Now}"); }
        }
        public async Task<IEnumerable<MarginAsset>> GetMarginAssetsAsync()
        {
            try
            {
                return await _binanceRequest.GetMarginAssets();
            }
            catch { throw new Exception($"Ошибка в методе GetMarginAssetsAsync {DateTime.Now}"); }
        }

        #endregion

        #region Spot methods

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
        public async Task<NewOrder> TakeOrderStopLossLimit(Signal signal)
        {
            try
            {
                return await _binanceRequest.OrderStopLossLimit(signal.Symbol, signal.Quantity, signal.StopLimitPrice, signal.StopLoss, signal.Side);
            }
            catch { throw new Exception($"Ошибка в методе TakeOrderStopLoss ({DateTime.Now})"); }

        }
        public async Task<CanceledOrder> CancelLimitOrder(Order order)
        {
            try
            {
                return await _binanceRequest.CancelOrder(order.Symbol, order.OrderId);
            }
            catch { throw new Exception($"Ошибка в методе CancelLimitOrder ({DateTime.Now})"); }
        }

        public async Task<Order> GetLimitOrder(NewOrder order)
        {
            try
            {
                return await _binanceRequest.GetOrder(order.Symbol, order.OrderId, order.ClientOrderId);
            }
            catch { throw new Exception($"Ошибка в методе GetLimitOrder ({DateTime.Now})"); }
        }

        public async Task<IEnumerable<Order>> GetCurrentOpenOrders(string symbol)
        {
            try
            {
                return await _binanceRequest.GetCurrentOpenOrders(symbol);
            }
            catch { throw new Exception($"Ошибка в методе GetCurrentOpenOrders ({DateTime.Now})"); }
        }
        public async Task<List<QueryOrder>> GetOpenOCO(string symbol)
        {
            try
            {
                List<OpenOCOOrder> opensOrders = await _binanceRequest.QueryOpenOCO();

                return opensOrders.SelectMany(o => o.Orders.Where(x => x.Symbol.Equals(symbol))).ToList();
            }
            catch { throw new Exception($"Ошибка в методе GetOpenOCO ({DateTime.Now})"); }
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
        public async Task<IEnumerable<Candlestick>> GetCandlestickAsync(string symbol, TimeInterval timeInterval, DateTime? startTime = null, DateTime? endTime = null, int quantity = 50)
        {
            return await _binanceRequest.GetCandleSticks(symbol, timeInterval, startTime, endTime, limit: quantity);
        }
        public async Task<decimal> GetLastBuyPriceAsync(string symbol)
        {
            Trade lastTrade = await GetLastTrade(symbol);

            return lastTrade.Price;
        }
        public async Task<decimal> GetCurrentPrice(string symbol, TimeInterval timeInterval)
        {
            IEnumerable<Candlestick> candle = await GetCandlestickAsync(symbol, timeInterval, quantity: 1);

            return candle.Last().Close;
        }
        public async Task TrailingOCO(Signal signal, TimeInterval timeInterval)
        {
            Console.WriteLine("TrailingOCO");

            OCOOrder orderOco = await PostOrderOCOAsync(signal);

            await Task.Delay(2500);

            for (uint i = 0; i < uint.MaxValue; i++)
            {
                try
                {
                    QueryOCO runningOrder = await GetOcoOrderAsync(orderOco);

                    if (!runningOrder.ListOrderStatus.Equals("ALL_DONE"))
                    {
                        IEnumerable<Candlestick> candlestick = await GetCandlestickAsync(signal.Symbol, timeInterval, quantity: 1);

                        if (signal.Price - candlestick.First().Close <= 0.2m)
                        {                        
                            QueryOCO canceledOrder = await _binanceRequest.CancelOCO(signal.Symbol, orderOco.ListClientOrderId);

                            await Task.Delay(2000);

                            ReformSignal(signal);

                            orderOco = await PostOrderOCOAsync(signal);
                        }
                    }
                    else { break; }
                    await Task.Delay(2000);
                }
                catch (Exception e) { Console.WriteLine(e.Message); continue; }
            }
        }

        #endregion

        private void ReformSignal(Signal signal)
        {
            signal.Price += 0.2m;
            signal.StopLoss += 0.2m;
            signal.StopLimitPrice += 0.2m;
        }  
    }
}
