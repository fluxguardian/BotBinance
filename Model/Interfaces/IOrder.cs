using Model.Enums;
using Model.Models.Account.Spot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface IOrder
    {
        Task<IEnumerable<Order>> GetAllOrders(string symbol, long? orderId = null, int limit = 500, long recvWindow = 5000);
        Task<IEnumerable<Order>> GetCurrentOpenOrders(string symbol, long recvWindow = 5000);
        Task<CanceledOrder> CancelOrder(string symbol, long? orderId = null, string origClientOrderId = null, long recvWindow = 5000);
        Task<Order> GetOrder(string symbol, long? orderId = null, string origClientOrderId = null, long recvWindow = 5000);
        Task<NewOrder> PostNewOrder(string symbol, decimal quantity, decimal price, OrderSide side, OrderType orderType, TimeInForce timeInForce,
            decimal icebergQty = 0m, long recvWindow = 5000);
        Task<OCOOrder> PostOCOOrder(string symbol, decimal quantity, decimal price, decimal stopPrice, decimal stopLimitPrice,
            OrderSide side, TimeInForce timeInForce = TimeInForce.GTC, long recvWindow = 5000);
        Task<QueryOCO> QueryOCO(long orderListId);
        Task<NewOrder> OrderQuantityMarket(string symbol, decimal quantity, OrderSide side);
        Task<NewOrder> OrderQuoteMarket(string symbol, decimal quoteOrderQty, OrderSide side);
        Task<QueryOCO> CancelOCO(string symbol, string listClientOrderId);
    }
}
