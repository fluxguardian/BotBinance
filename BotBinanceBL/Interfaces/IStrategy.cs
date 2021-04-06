using BotBinanceBL.Stocks.Interfaces;
using System.Threading.Tasks;

namespace BotBinanceBL.Interfaces
{
    public interface IStrategy
    {
        public void Trade(IStock stock);
        Task Logic();
    }
}
