using Model.Models.Market;
using System.Collections.Generic;
using System.Linq;
using Trends.TechnicalAnalysis;

namespace TechnicalAnalysis.Oscillators
{
    public class Stochastic : ISignal
    {
        public static StochasticValues GetStochastic(List<Candlestick> candlesticks, int period = 14, int smoothK = 3, int smoothD = 3)
        {
            decimal max = 0.0m;
            decimal min = 0.0m;

            List<decimal> K = new List<decimal>();

            for (int i = period; i < candlesticks.Count + 1; i++)
            {
                max = candlesticks.Skip(i - period).Take(period).Max(p => p.High);
                min = candlesticks.Skip(i - period).Take(period).Min(p => p.Low);

                if (max - min == 0.0m)
                {
                    K.Add(0.0m);
                }
                else
                {
                    K.Add((candlesticks[i - 1].Close - min) / (max - min) * 100);
                } 
            }

            var smoothedK = SimpleMovingAverage.GetSimpleMovingAverages(K, smoothK);
            var smoothedD = SimpleMovingAverage.GetSimpleMovingAverages(smoothedK, smoothD);

            return new StochasticValues() 
            { 
                SmoothD = smoothedD,
                SmoothK = smoothedK
            };
        }

        public bool BuySignal(IEnumerable<decimal> prices)
        {
            if(prices.SkipLast(1).Last() < 20.0m && prices.Last() > 20.0m)
            {
                return true;
            }
            return false;
        }

        public bool BuySignalCross(IEnumerable<decimal> prices)
        {
            throw new System.NotImplementedException();
        }

        public bool SellSignal(IEnumerable<decimal> prices)
        {
            if (prices.SkipLast(1).Last() > 80.0m && prices.Last() < 80.0m)
            {
                return true;
            }
            return false;
        }

        public bool SellSignalCross(IEnumerable<decimal> prices)
        {
            throw new System.NotImplementedException();
        }
    }

    public class StochasticValues
    {
        public List<decimal> SmoothK { get; set; }
        public List<decimal> SmoothD { get; set; }
    }
}
