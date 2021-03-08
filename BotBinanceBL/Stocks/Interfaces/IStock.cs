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
        Task<OCOOrder> PostOrderOCOAsync(Signal signal);
        Task<NewOrder> TakeMarketOrder(Signal signal);
        Task<QueryOCO> GetOcoOrderAsync(OCOOrder order);
        Task<QueryOCO> CanceledOrderAsync(string symbol, OCOOrder order);
        Task<AccountInfo> GetAccountInformation();
        Task<Symbol> GetExchangeInformationAsync(string symbol);
        Task TrailingOCO(Signal signal, TimeInterval timeInterval);
        Task<IEnumerable<Candlestick>> GetCandlestickAsync(string symbol, TimeInterval timeInterval, int quantity = 50);
        Task<decimal> GetLastBuyPriceAsync(string symbol);
        Task<decimal> GetCurrentPrice(string symbol, TimeInterval timeInterval);
        Task<AccountTransfer> TransferAsync(string asset, decimal amount, TransferType transferType);
        Task<AccountBorrow> BorrowAsync(string asset, decimal amount, string isIsolated = "FALSE");
        Task<AccountRepay> RepayAsync(string asset, decimal amount, string isIsolated = "FALSE");
        Task<MaxBorrow> MaxBorrowAsync(string asset);
        Task<OrderMargin> MarketOrderMarginAsync(string symbol, decimal quantity, OrderSide side, string isIsolated = "FALSE");
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
    }
}
