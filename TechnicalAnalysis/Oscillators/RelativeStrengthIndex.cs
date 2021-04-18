using Model.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TechnicalAnalysis.Oscillators
{
    public class RelativeStrengthIndex
    {
        public int Period { get; set; }
        private decimal _valueBuy { get; set; }
        private decimal _valueSell { get; set; }

        public RelativeStrengthIndex(int period)
        {
            Period = period;
        }
        public RelativeStrengthIndex()
        {

        }
        public RelativeStrengthIndex(int period, decimal valueBuy, decimal valueSell)
        {
            Period = period;
            _valueBuy = valueBuy;
            _valueSell = valueSell;
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
        public List<decimal> GetRSI(List<decimal> prices, int period)
        {
            List<decimal> up = new List<decimal>();
            List<decimal> down = new List<decimal>();

            for (int i = 0; i < prices.Take(period).Count(); i++)
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

            decimal avrLoss = down.Sum() / Convert.ToDecimal(period);
            decimal avrGain = up.Sum() / Convert.ToDecimal(period);

            decimal nextAvgGain = 0.0m;
            decimal nextAvgLoss = 0.0m;

            decimal currentGain = 0.0m;
            decimal currentLoss = 0.0m;

            List<decimal> rsiLst = new List<decimal>();

            for (int j = period; j < prices.Count - 1; j++)
            {
                if (prices[j + 1] - prices[j] > 0)
                {
                    currentGain = prices[j + 1] - prices[j];
                }
                else if (prices[j + 1] - prices[j] < 0)
                {
                    currentLoss = (prices[j + 1] - prices[j]) * -1;
                }

                nextAvgGain = ((avrGain * Convert.ToDecimal(period - 1)) + currentGain) / Convert.ToDecimal(period);
                nextAvgLoss = ((avrLoss * Convert.ToDecimal(period - 1)) + currentLoss) / Convert.ToDecimal(period);

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

        public bool IsBuy(List<Candlestick> candlesticks)
        {
            var rsi = GetRSI(candlesticks.Select(x => x.Close).ToList());

            if (rsi.Last() < _valueBuy)
            {
                return true;
            }

            return false;
        }
        public bool IsSell(List<Candlestick> candlesticks)
        {
            var rsi = GetRSI(candlesticks.Select(x => x.Close).ToList());

            if (rsi.Last() > _valueSell)
            {
                return true;
            }

            return false;
        }

        public bool IsBuy(List<Candlestick> candlesticks, decimal value, params int[] periods)
        {
            var rsi = GetRSI(candlesticks.Select(x => x.Close).ToList(), periods.First());

            if (rsi.Last() < value)
            {
                return true;
            }

            return false;
        }
        public bool IsSell(List<Candlestick> candlesticks, decimal value, params int[] periods)
        {
            var rsi = GetRSI(candlesticks.Select(x => x.Close).ToList(), periods.First());

            if (rsi.Last() > value)
            {
                return true;
            }

            return false;
        }
    }
}

