using Model.Models.Account.Spot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface IAccount
    {
        Task<AccountInfo> GetAccountInfoAsync(long recvWindow = 5000);
        Task<IEnumerable<Trade>> GetTradeList(string symbol, int limit = 500, long recvWindow = 5000);
        Task<IEnumerable<Order>> GetAllOrders(string symbol, long? orderId = null, int limit = 500, long recvWindow = 5000);
    }
}
