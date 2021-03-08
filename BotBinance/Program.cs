using BotBinanceBL;
using BotBinanceBL.Keys;
using BotBinanceBL.Stocks;
using Model.Enums;
using Strategy;
using System;
using System.Collections.Generic;

namespace BotBinance
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client(new Binance(Settings.Key, Settings.SecretKey));

            //client.AddStrategy(new ScalpSMA("LINKUSDT", TimeInterval.Minutes_3, "flaxmine"));
            client.AddStrategy(new RsiSignal(new List<string>()
            { 
                "LINKUSDT", "UNIUSDT", "XRPUSDT", "ETHUSDT", "BNBUSDT", "DOGEUSDT", "ADAUSDT", "XRPUSDT"
            }, 
            10, TimeInterval.Minutes_3));
            client.StartStrategies();

            //Client clientKirill = new Client(new Binance(Settings.KirillKey, Settings.KirillSecretKey));

            //clientKirill.AddStrategy(new Scalp("LINKUSDT", TimeInterval.Minutes_3, "Kirill"));
            //clientKirill.StartStrategies();

            Console.ReadKey();
        }
    }
}
