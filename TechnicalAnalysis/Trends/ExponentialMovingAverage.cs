using System;
using System.Collections.Generic;
using System.Linq;

namespace Trends.TechnicalAnalysis
{
    public static class ExponentialMovingAverage
    {
        public static List<decimal> GetExponentialMovingAverages(List<decimal> candlesticks, int period = 14)
        {
            List<decimal> emaValues = new List<decimal>();

            decimal alpha = 2 / Convert.ToDecimal(period + 1);
            decimal SMA = candlesticks.Take(period).Average();

            decimal EMA = (candlesticks[period + 1] - SMA) * alpha + SMA;

            for (int i = period + 1; i < candlesticks.Count - 1; i++)
            {
                EMA = (candlesticks[i + 1] - EMA) * alpha + EMA;
                emaValues.Add(EMA);
            }
            return emaValues;
        }

        public static decimal GetNextValueEMA(decimal previousEMA, decimal price, int period = 14)
        {
            decimal aplha = 2 / Convert.ToDecimal(period + 1);

            return (price - previousEMA) * aplha + previousEMA;
        }
    }
}
