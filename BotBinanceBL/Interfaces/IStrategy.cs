using BotBinanceBL.Stocks.Interfaces;

namespace BotBinanceBL.Interfaces
{
    public interface IStrategy
    {
        public void Trade(IStock stock);
    }
}
