using BotBinanceBL;
using BotBinanceBL.Keys;
using BotBinanceBL.Stocks;
using Model.Enums;
using Strategy;
using System;
using System.Collections.Generic;
using TechnicalAnalysis.Interfaces;
using TechnicalAnalysis.Oscillators;
using TechnicalAnalysis.Trends;

namespace BotBinance
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<IIndicator> indicators = new List<IIndicator>()
            //{
            //    new RelativeStrengthIndex(15, valueBuy: 30, valueSell: 70),
            //    new TrueStrengthIndex(25, 13, 13, valueBuy: -0.1m, valueSell: 0.1m)
            //};

            //List<string> symbols = new List<string>()
            //{
            //    "BNBUSDT", "XRPUSDT", "NEOUSDT", "ETHUSDT",
            //    "LINKUSDT", "ONEUSDT", "ANKRUSDT", "SCUSDT",
            //    "VETUSDT", "BCHUSDT", "ETCUSDT", "BTTUSDT",
            //    "XEMUSDT", "MANAUSDT", "DASHUSDT", "ZENUSDT",
            //    "LTCUSDT", "RLCUSDT", "BTCUSDT", "DOTUSDT",
            //    "DENTUSDT", "ONTUSDT", "IOSTUSDT",
            //    "TRBUSDT", "ICXUSDT", "DASHUSDT", "ZILUSDT",
            //    "EOSUSDT", "YFIUSDT", "NEARUSDT", "LUNAUSDT",
            //    "UNFIUSDT", "COMPUSDT"
            //};

            //Client client = new Client(new Binance(Settings.Key, Settings.SecretKey));  

            //client.AddStrategy(new IndicatorSignal(indicators, symbols, TimeInterval.Minutes_5));
            //client.StartStrategies();

            Client client = new Client(new Binance(Settings.Key, Settings.SecretKey));

            List<IIndicator> indicators = new List<IIndicator>()
            {
                new RelativeStrengthIndex(),
                new TrueStrengthIndex(),
                new LinearRegression()
            };  

            client.AddStrategy(new LastStrategy(indicators, TimeInterval.Minutes_5));
            client.StartStrategies();

            Console.ReadKey();
        }
    }
}
