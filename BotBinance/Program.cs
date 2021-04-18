using BotBinanceBL;
using BotBinanceBL.Keys;
using BotBinanceBL.Stocks;
using Csv;
using Model.Enums;
using Strategy;
using Strategy.Data.DataLastStrategy;
using System;
using System.Collections.Generic;
using System.IO;

namespace BotBinance
{
    class Program
    {
        private static string appPath => Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        private static string _path => appPath + @"\Data\DataLastStrategy\DataOfIndicators.csv";

        static void Main(string[] args)
        {
            #region Singals

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

            #endregion

            List<DataStrategy> dataStrategies = CSV<DataStrategy>.ReadCsv(_path);        

            Client client = new Client(new Binance(Settings.Key, Settings.SecretKey));

            client.AddStrategy(new LastStrategy(dataStrategies, TimeInterval.Minutes_5));
            client.StartStrategies();

            Console.ReadKey();
        }
    }
}
