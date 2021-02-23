using Model.Models.Market;
using System.Collections.Generic;
using System.Linq;
using Trends.TechnicalAnalysis;

namespace TechnicalAnalysis.Oscillators
{
    public static class StochasticRSI
    {
        public static StochasticRSIValues GetStochasticRSI(IEnumerable<Candlestick> candlesticks, 
            int periodRSI = 14, int periodStoch = 14, int smoothK = 3, int smoothD = 3)
        {
            decimal max = 0.0m;
            decimal min = 0.0m;

            List<decimal> rsi = RelativeStrengthIndex.GetRSI(candlesticks.Select(p => p.Close).ToList(), periodRSI);
            List<decimal> stochasticRSI = new List<decimal>();

            for (int i = periodStoch; i < rsi.Count + 1; i++)
            {
                max = rsi.Skip(i - periodStoch).Take(periodStoch).Max();
                min = rsi.Skip(i - periodStoch).Take(periodStoch).Min();

                stochasticRSI.Add((rsi[i - 1] - min) / (max - min) * 100);
            }

            List<decimal> smoothedK = SimpleMovingAverage.GetSimpleMovingAverages(stochasticRSI, smoothK);
            List<decimal> smoothedD = SimpleMovingAverage.GetSimpleMovingAverages(smoothedK, smoothD);

            return new StochasticRSIValues()
            {
                SmoothD = smoothedD,
                SmoothK = smoothedK
            };
        }
    }

    public class StochasticRSIValues
    {
        public List<decimal> SmoothK { get; set; }
        public List<decimal> SmoothD { get; set; }
    }
}
