using BotBinanceBL.Interfaces;
using BotBinanceBL.Stocks.Interfaces;
using Model.Enums;
using Model.TradingRules;
using Strategy.TradeSettings;
using System;
using System.Linq;
using TechnicalAnalysis.Oscillators;
using TechnicalAnalysis.Volatility;
using Trends.TechnicalAnalysis;

namespace Strategy
{
    public class ScalpSMA : IStrategy
    {
        private IStock _stock { get; set; }
        private string _name { get; set; }
        private string _symbol { get; set; }
        private TimeInterval _timeInterval { get; set; }
        private SimpleMovingAverage _sma { get; set; }
        private TrueStrengthIndex _tsi { get; set; }
        private Normalization _normalization { get; set; }
        private Symbol _asset { get; set; }

        public ScalpSMA(string symbol, TimeInterval timeInterval, string name)
        {
            _symbol = symbol;
            _timeInterval = timeInterval;
            _name = name;

            _sma = new SimpleMovingAverage(shortPeriod: 90, longPeriod: 200);
            _tsi = new TrueStrengthIndex(25, 13, 13);
        }

        public void Trade(IStock stock)
        {
            _stock = stock;
        }
    }
}
