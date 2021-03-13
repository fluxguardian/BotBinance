using Model.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TechnicalAnalysis.Volatility
{
    public class AverageTrueRange
    {
        public int Period { get; set; }

        public AverageTrueRange(int period)
        {
            Period = period;
        }

        public List<decimal> GetATR(List<Candlestick> candlesticks)
        {
            decimal atr = GetFirstATR(candlesticks);
            List<decimal> lstValues = new List<decimal>();

            lstValues.Add(atr);

            for (int i = Period + 1; i < candlesticks.Count; i++)
            {
                decimal trueRange = Math.Max(Math.Abs(candlesticks[i].Low - candlesticks[i - 1].Close),
                    Math.Max(Math.Abs(candlesticks[i].High - candlesticks[i - 1].Close), candlesticks[i].High - candlesticks[i].Low));

                atr = (atr * (Period - 1) + trueRange) / Period;

                lstValues.Add(atr);
            }
            return lstValues;
        }

        private decimal GetFirstATR(List<Candlestick> candlesticks)
        {
            var trueRange = new List<decimal>();

            for (int i = 1; i <= Period; i++)
            {
                trueRange.Add(Math.Max(
                    Math.Abs(candlesticks[i].Low - candlesticks[i - 1].Close),
                        Math.Max(
                            Math.Abs(candlesticks[i].High - candlesticks[i - 1].Close), candlesticks[i].High - candlesticks[i].Low)));
            }
            return trueRange.Average();
        }
    }
}
