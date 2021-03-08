using Model.Enums;
using Model.Models.Account.Margin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface IMargin
    {
        Task<AccountTransfer> Transfer(string asset, decimal amount, TransferType transferType);
        Task<AccountBorrow> Borrow(string asset, decimal amount, string isIsolated = "FALSE");
        Task<AccountRepay> Repay(string asset, decimal amount, string isIsolated = "FALSE");
        Task<MaxBorrow> MaxBorrow(string asset);
        Task<OrderMargin> MarketOrderQuantityMargin(string symbol, decimal quantity, OrderSide side, string isIsolated = "FALSE");
        Task<OrderMargin> MarketOrderQuoteMargin(string symbol, decimal quantity, OrderSide side, string isIsolated = "FALSE");
        Task<OrderMargin> OrderMarginStopLoss(string symbol, decimal quantity, decimal price, decimal stopPrice,
            OrderSide orderSide, OrderType orderType, TimeInForce timeInForce = TimeInForce.GTC);
        Task<MaxTransferOutAmount> MaxTransferOutAmount(string asset, string isIsolated = "FALSE");
        Task<CanceledOrderMargin> CancelOrderMargin(string symbol, string origClientOrderId);
        Task<QueryMarginOrder> QueryMarginOrder(string symbol, string origClientOrderId, string isIsolated = "FALSE");
        Task<QueryRepay> QueryRepayRecord(string asset, DateTime startTime, DateTime? endTime = null, long size = 10);
        Task<QueryMarginAccountDetails> MarginAccountDetails();
        Task<IEnumerable<MarginTradeList>> MarginTradeLists(string symbol, DateTime? startTime = null, DateTime? endTime = null, 
            int? fromId = null, string isIsolated = "FALSE", int limit = 500);
        Task<IEnumerable<MarginAsset>> GetMarginAssets();
    }
}
