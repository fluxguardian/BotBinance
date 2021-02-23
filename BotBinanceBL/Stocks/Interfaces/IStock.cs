using BotBinanceBL.Stocks.Signals;
using Model.Enums;
using Model.Models.Account;
using Model.Models.Market;
using Model.TradingRules;
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
     }
}
