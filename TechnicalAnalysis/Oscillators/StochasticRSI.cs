using Model.Models.Market;
using System.Collections.Generic;
using System.Linq;
using Trends.TechnicalAnalysis;

namespace TechnicalAnalysis.Oscillators
{
    public class StochasticRSI
    {
        public int Period { get; set; }
        public int PeriodStoch { get; set; }
        public int SmoothK { get; set; }
        public int SmoothD { get; set; }
        private RelativeStrengthIndex _rsi { get; set; }
        public StochasticRSI(int period, int periodStoch, int smoothK, int smoothD)
        {
            Period = period;
            PeriodStoch = periodStoch;
            SmoothK = smoothK;
            SmoothD = smoothD;

            _rsi = new RelativeStrengthIndex(Period);
        }

        public StochasticRSIValues GetStochasticRSI(IEnumerable<Candlestick> candlesticks)
        {
            decimal max = 0.0m;
            decimal min = 0.0m;

            List<decimal> rsi = _rsi.GetRSI(candlesticks.Select(p => p.Close).ToList());
            List<decimal> stochasticRSI = new List<decimal>();

            for (int i = PeriodStoch; i < rsi.Count + 1; i++)
            {
                max = rsi.Skip(i - PeriodStoch).Take(PeriodStoch).Max();
                min = rsi.Skip(i - PeriodStoch).Take(PeriodStoch).Min();

                stochasticRSI.Add((rsi[i - 1] - min) / (max - min) * 100);
            }

            List<decimal> smoothedK = SimpleMovingAverage.GetSimpleMovingAverages(stochasticRSI, SmoothK);
            List<decimal> smoothedD = SimpleMovingAverage.GetSimpleMovingAverages(smoothedK, SmoothD);

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
