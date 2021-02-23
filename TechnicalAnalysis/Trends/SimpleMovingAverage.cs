using System.Collections.Generic;
using System.Linq;
using TechnicalAnalysis;

namespace Trends.TechnicalAnalysis
{
    public class SimpleMovingAverage : ISignal
    {
        public int ShortPeriod { get; set; }
        public int LongPeriod { get; set; }

        public SimpleMovingAverage(int shortPeriod, int longPeriod)
        {
            ShortPeriod = shortPeriod;
            LongPeriod = longPeriod;
        }

        public static List<decimal> GetSimpleMovingAverages(List<decimal> candlesticks, int period)
        {
            List<decimal> lstAverages = new List<decimal>();

            for (int i = period; i < candlesticks.Count + 1; i++)
            {
                lstAverages.Add(candlesticks.Skip(i - period).Take(period).Average());
            }

            return lstAverages;
        }

        public static decimal GetSimpleMovingAverage(List<decimal> candlesticks, int period)
        {
            return candlesticks.TakeLast(period).Average();
        }

        public bool BuySignalCross(IEnumerable<decimal> prices)
        {
            var shortAverage = GetSimpleMovingAverages(prices.ToList(), ShortPeriod);
            var longAverage = GetSimpleMovingAverages(prices.ToList(), LongPeriod);

            if (shortAverage.SkipLast(1).Last() < longAverage.SkipLast(1).Last()
                && shortAverage.Last() > longAverage.Last())
            {
                return true;
            }
            return false;
        }

        public bool SellSignalCross(IEnumerable<decimal> prices)
        {
            var shortAverage = GetSimpleMovingAverages(prices.ToList(), ShortPeriod);
            var longAverage = GetSimpleMovingAverages(prices.ToList(), LongPeriod);

            if (shortAverage.SkipLast(1).Last() > longAverage.SkipLast(1).Last()
                && shortAverage.Last() < longAverage.Last())
            {
                return true;
            }
            return false;
        }

        public bool BuySignal(IEnumerable<decimal> prices)
        {
            decimal shortAverage = GetSimpleMovingAverage(prices.ToList(), ShortPeriod);
            decimal longAverage = GetSimpleMovingAverage(prices.ToList(), LongPeriod);

            if(shortAverage > longAverage)
            {
                return true;
            }
            return false;
        }

        public bool SellSignal(IEnumerable<decimal> prices)
        {
            decimal shortAverage = GetSimpleMovingAverage(prices.ToList(), ShortPeriod);
            decimal longAverage = GetSimpleMovingAverage(prices.ToList(), LongPeriod);

            if (shortAverage < longAverage)
            {
                return true;
            }
            return false;
        }
    }
}
