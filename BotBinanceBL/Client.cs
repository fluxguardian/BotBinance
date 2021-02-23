using BotBinanceBL.Interfaces;
using BotBinanceBL.Stocks.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotBinanceBL
{
    public class Client
    {
        private IStock _stock { get; set; }
        private IStrategy _strategy { get; set; }
        private List<IStrategy> _strategies { get; set; }

        public Client(IStock stock)
        {
            _strategies = new List<IStrategy>();
            _stock = stock;
        }

        public void AddStrategy(IStrategy strategy)
        {
            _strategies.Add(strategy);
        }

        public void StartStrategies()
        {
            foreach (IStrategy strategy in _strategies)
            {
                new Task(() => strategy.Trade(_stock)).Start();
            }
        }

        public void SetStrategy(IStrategy strategy)
        {
            _strategy = strategy;
        }

        public void StartTrade()
        {
            new Task(() => _strategy.Trade(_stock)).Start();
        }
    }
}
