using BotBinanceBL;
using BotBinanceBL.Keys;
using BotBinanceBL.Stocks;
using Model.Enums;
using Strategy;
using System;

namespace BotBinance
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client(new Binance(Settings.Key, Settings.SecretKey));

            client.AddStrategy(new Scalp("LINKUSDT", TimeInterval.Minutes_3, "flaxmine"));
            client.StartStrategies();

            Console.ReadKey();
        }
    }
}
