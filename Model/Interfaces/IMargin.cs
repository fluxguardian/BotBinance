using Model.Enums;
using Model.Models.Account;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface IMargin
    {
        Task<AccountTransfer> Transfer(string asset, decimal amount, TransferType transferType);
        Task<AccountBorrow> Borrow(string asset, decimal amount, string isIsolated = "FALSE");
        Task<AccountRepay> Repay(string asset, decimal amount, string isIsolated = "FALSE");
        Task<MaxBorrow> MaxBorrow(string asset);
        Task<NewOrderMargin> MarketOrderQuantityMargin(string symbol, decimal quantity, OrderSide side, string isIsolated = "FALSE");
        Task<NewOrderMargin> MarketOrderQuoteMargin(string symbol, decimal quantity, OrderSide side, string isIsolated = "FALSE");
        Task<MaxTransferOutAmount> MaxTransferOutAmount(string asset, string isIsolated = "FALSE");
    }
}
