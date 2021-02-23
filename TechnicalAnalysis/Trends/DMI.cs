using Model.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trends.TechnicalAnalysis;

namespace TechnicalAnalysis.Trends
{
    public class DMI
    {
        private int PeriodMovingAverage { get; set; }
        public DMI(int periodSMA) 
        {
            PeriodMovingAverage = periodSMA;
        }

        public DMI() { }

        public decimal GetDMI(List<Candlestick> candlesticks)
        {
            List<decimal> trueRange = new List<decimal>();
            List<decimal> predplusDI = new List<decimal>();

            List<decimal> plusDM = new List<decimal>();
            List<decimal> minusDM = new List<decimal>();

            for (int i = candlesticks.Count - 1, j = 0; i > 0; i--, j++)
            {
                plusDM.Add(candlesticks[i].High > candlesticks[i - 1].High ? candlesticks[i].High - candlesticks[i - 1].High : 0);
                minusDM.Add(candlesticks[i].Low < candlesticks[i - 1].Low ? candlesticks[i - 1].Low - candlesticks[i].Low : 0);

                if(plusDM[j] > minusDM[j]) { minusDM[j] = 0; }
                if(minusDM[j] > plusDM[j]) { plusDM[j] = 0; }
                if(plusDM[j] == minusDM[j]) { plusDM[j] = 0; minusDM[j] = 0; }

                trueRange.Add(
                    new List<decimal>()
                    {
                        Math.Abs(candlesticks[i].Low - candlesticks[i - 1].Close),
                        Math.Abs(candlesticks[i].High - candlesticks[i - 1].Close),
                        candlesticks[i].High - candlesticks[i].Low
                    }.Max());
            }

            var emaTrueRange = SimpleMovingAverage.GetSimpleMovingAverage(trueRange, 14);
            var emaPlusDM = SimpleMovingAverage.GetSimpleMovingAverage(plusDM, 14);

            decimal plusDI = emaPlusDM / emaTrueRange;


            return plusDI;
        }
    }
}
