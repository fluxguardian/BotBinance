using System;
using System.Collections.Generic;
using System.Linq;

namespace TechnicalAnalysis.Oscillators
{
    public class RelativeStrengthIndex
    {
        public int Period { get; set; }
        public RelativeStrengthIndex(int period)
        {
            Period = period;
        }

        public List<decimal> GetRSI(List<decimal> prices)
        {
            List<decimal> up = new List<decimal>();
            List<decimal> down = new List<decimal>();

            for (int i = 0; i < prices.Take(Period).Count(); i++)
            {
                if (prices[i + 1] - prices[i] < 0)
                {
                    down.Add((prices[i + 1] - prices[i]) * -1);
                }
                else if (prices[i + 1] - prices[i] > 0)
                {
                    up.Add(prices[i + 1] - prices[i]);
                }
            }

            decimal avrLoss = down.Sum() / Convert.ToDecimal(Period);
            decimal avrGain = up.Sum() / Convert.ToDecimal(Period);

            decimal nextAvgGain = 0.0m;
            decimal nextAvgLoss = 0.0m;

            decimal currentGain = 0.0m;
            decimal currentLoss = 0.0m;

            List<decimal> rsiLst = new List<decimal>();

            for (int j = Period; j < prices.Count - 1; j++)
            {
                if (prices[j + 1] - prices[j] > 0)
                {
                    currentGain = prices[j + 1] - prices[j];
                }
                else if (prices[j + 1] - prices[j] < 0)
                {
                    currentLoss = (prices[j + 1] - prices[j]) * -1;
                }

                nextAvgGain = ((avrGain * Convert.ToDecimal(Period - 1)) + currentGain) / Convert.ToDecimal(Period);
                nextAvgLoss = ((avrLoss * Convert.ToDecimal(Period - 1)) + currentLoss) / Convert.ToDecimal(Period);

                avrGain = nextAvgGain;
                avrLoss = nextAvgLoss;

                currentGain = 0.0m;
                currentLoss = 0.0m;

                if (nextAvgLoss == 0.0m)
                {
                    rsiLst.Add(100.0m);
                }
                else
                {
                    rsiLst.Add(100.0m - (100.0m / (1.0m + (nextAvgGain / nextAvgLoss))));
                }
            }

            return rsiLst;
        }
    }
}

