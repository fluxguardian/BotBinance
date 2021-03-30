using System;
using System.Collections.Generic;
using System.Linq;

namespace TechnicalAnalysis.Trends
{
    public class LinearRegression
    {
        public int ShortPeriod { get; set; }
        public int LongPeriod { get; set; }
        public LinearRegression(int shortPeriod, int longPeriod)
        {
            ShortPeriod = shortPeriod;
            LongPeriod = longPeriod;
        }
        public LinearRegression()
        {

        }

        public List<LinearRegressionCurve> GetValuesCurve(List<decimal> prices, int period)
        {
            List<LinearRegressionCurve> valuesCurve = new List<LinearRegressionCurve>();

            for (int i = 0; i <= prices.Count - period; i++)
            {
                List<decimal> y_prices = prices.Skip(i).Take(period).ToList();

                valuesCurve.Add(GetValueCurve(y_prices, period));
            }

            return valuesCurve;
        }
        public LinearRegressionCurve GetValueCurve(List<decimal> prices, int period)
        {
            List<decimal> y_prices = prices.TakeLast(period).ToList();

            decimal Sx = 0;
            decimal Sy = 0;
            decimal Sxx = 0;
            decimal Sxy = 0;

            double sum = 0;

            for (int x = 1; x <= period; x++)
            {
                Sx += x;
                Sy += y_prices[x - 1];
                Sxx += x * x;
                Sxy += x * y_prices[x - 1];
            }

            #region Коэффициенты регресси

            decimal a = (period * Sxy - Sx * Sy) / (period * Sxx - Sx * Sx);
            decimal b = (Sy - a * Sx) / period;

            decimal linearValue = a * period + b; // y = 4.11*x+59943.76 // y = 2.97x+60011

            #endregion

            #region Среднеквадратическое отклонение

            y_prices.ForEach(x => sum += Math.Pow(Convert.ToDouble(x - linearValue), 2));

            decimal stdDev = Convert.ToDecimal(Math.Sqrt(sum / period));
            //decimal slope = a * 100.0m / y_prices.Last();
            decimal slope = a * 100.0m;
            #endregion

            return new LinearRegressionCurve()
            {
                ValueCurve = linearValue,
                Slope = slope,
                Angle = Convert.ToDecimal(Math.Atan(Convert.ToDouble(slope / y_prices.Last())) * (180 / Math.PI)),
                LinearRegressionBands = new LinearRegressionBands()
                {
                    UpLine = linearValue + (2.0m * stdDev),
                    DownLine = linearValue - (2.0m * stdDev),
                    Width = (linearValue + (2.0m * stdDev) - (linearValue - (2.0m * stdDev))) / linearValue
                }
            };
        }

        public bool BuySignalCross(IEnumerable<decimal> prices)
        {
            List<LinearRegressionCurve> shortLR = GetValuesCurve(prices.ToList(), ShortPeriod);
            List<LinearRegressionCurve> longLR = GetValuesCurve(prices.ToList(), LongPeriod);

            if (shortLR.SkipLast(1).Last().ValueCurve < longLR.SkipLast(1).Last().ValueCurve
                && shortLR.Last().ValueCurve > longLR.Last().ValueCurve)
            {
                return true;
            }
            return false;
        }
        public bool SellSignalCross(IEnumerable<decimal> prices)
        {
            List<LinearRegressionCurve> shortLR = GetValuesCurve(prices.ToList(), ShortPeriod);
            List<LinearRegressionCurve> longLR = GetValuesCurve(prices.ToList(), LongPeriod);

            if (shortLR.SkipLast(1).Last().ValueCurve > longLR.SkipLast(1).Last().ValueCurve
                && shortLR.Last().ValueCurve < longLR.Last().ValueCurve)
            {
                return true;
            }
            return false;
        }
        public bool BuySignal(List<decimal> prices)
        {
            LinearRegressionCurve shortAverage = GetValuesCurve(prices, ShortPeriod).SkipLast(1).Last();
            LinearRegressionCurve longAverage = GetValuesCurve(prices, LongPeriod).SkipLast(1).Last();

            if (shortAverage.ValueCurve > longAverage.ValueCurve)
            {
                return true;
            }
            return false;
        }
        public bool SellSignal(List<decimal> prices)
        {
            LinearRegressionCurve shortAverage = GetValuesCurve(prices, ShortPeriod).SkipLast(1).Last();
            LinearRegressionCurve longAverage = GetValuesCurve(prices, LongPeriod).SkipLast(1).Last();

            if (shortAverage.ValueCurve < longAverage.ValueCurve)
            {
                return true;
            }
            return false;
        }
    }

    public class LinearRegressionCurve
    {
        public decimal ValueCurve { get; set; }
        public decimal Slope { get; set; }
        public decimal Angle { get; set; }
        public LinearRegressionBands LinearRegressionBands { get; set; }
    }
    public class LinearRegressionBands
    {
        public decimal UpLine { get; set; }
        public decimal DownLine { get; set; }
        public decimal Width { get; set; }
    }
}
