using Model.Enums;
using Model.Interfaces;
using Model.TradingRules;
using System.Threading.Tasks;

namespace BotBinanceBL.Stocks.Interfaces
{
    public interface IRequest : IOrder, ICandle, IAccount
    {
        Task<T> CallAsync<T>(ApiMethod method, string endpoint, bool isSigned = false, string parameters = null);
        Task<TradingRules> GetExchangeInformationAsync();
    }
}
