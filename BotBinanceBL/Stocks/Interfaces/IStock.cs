using BotBinanceBL.Stocks.Signals;
using Model.Enums;
using Model.Models.Account.Margin;
using Model.Models.Account.Spot;
using Model.Models.Market;
using Model.TradingRules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotBinanceBL.Stocks.Interfaces
{
    public interface IStock
    {
        Task<List<Balance>> GetBalance(Symbol asset);
        Task<Trade> GetLastTrade(string symbol);
        Task<CanceledOrder> CancelLimitOrder(Order order);
        Task<Order> GetLimitOrder(NewOrder order);
        Task<OCOOrder> PostOrderOCOAsync(Signal signal);
        Task<NewOrder> TakeMarketOrder(Signal signal);
        Task<NewOrder> TakeOrderStopLossLimit(Signal signal);
        Task<IEnumerable<Order>> GetCurrentOpenOrders(string symbol);
        Task<QueryOCO> GetOcoOrderAsync(OCOOrder order);
        Task<QueryOCO> CanceledOrderAsync(string symbol, OCOOrder order);
        Task<AccountInfo> GetAccountInformation();
        Task<Symbol> GetExchangeInformationAsync(string symbol);
        Task TrailingOCO(Signal signal, TimeInterval timeInterval);
        Task<IEnumerable<Candlestick>> GetCandlestickAsync(string symbol, TimeInterval timeInterval, DateTime? startTime = null, DateTime? endTime = null, int quantity = 50);
        Task<decimal> GetLastBuyPriceAsync(string symbol);
        Task<decimal> GetCurrentPrice(string symbol, TimeInterval timeInterval);
        Task<AccountTransfer> TransferAsync(string asset, decimal amount, TransferType transferType);
        Task<decimal> GetMaxBorrowAsset(string asset);
        Task<AccountBorrow> BorrowBaseAsset(string asset, decimal amountBorrow);
        Task<AccountBorrow> BorrowQuoteAsset(string asset, decimal amountBorrow);
        Task<AccountRepay> RepayQuoteAsset(string asset);
        Task<AccountRepay> RepayBaseAsset(string asset);
        Task<OrderMargin> MarketOrderQuantityMarginAsync(string symbol, decimal quantity, OrderSide side, string isIsolated = "FALSE");
        Task<OrderMargin> MarketOrderQuoteMarginAsync(string symbol, decimal quoteOrderQty, OrderSide side, string isIsolated = "FALSE");
        Task<OrderMargin> OrderMarginStopLossAsync(string symbol, decimal quantity, decimal price, decimal stopPrice,
            OrderSide orderSide, OrderType orderType, TimeInForce timeInForce = TimeInForce.GTC);
        Task<MaxTransferOutAmount> MaxTransferOutAmountAsync(string asset, string isIsolated = "FALSE");
        Task<CanceledOrderMargin> CancelOrderMarginAsync(OrderMargin orderMargin);
        Task<QueryMarginOrder> QueryMarginOrderAsync(OrderMargin orderMargin);
        Task<QueryRepay> QueryRepayRecordAsync(string asset, DateTime startTime, DateTime? endTime = null, long size = 10);
        Task<QueryMarginAccountDetails> MarginAccountDetailsAsync();
        Task<IEnumerable<MarginTradeList>> MarginTradeListsAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null,
            int? fromId = null, string isIsolated = "FALSE", int limit = 500);
        Task<IEnumerable<MarginAsset>> GetMarginAssetsAsync();
        Task<List<QueryOrder>> GetOpenOCO(string symbol);
    }
}
