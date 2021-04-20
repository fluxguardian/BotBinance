using Model.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using TechnicalAnalysis.Interfaces;
using Trends.TechnicalAnalysis;

namespace TechnicalAnalysis.Oscillators
{
    public class TrueStrengthIndex : ISignal, IIndicator
    {
        private int _quantityCandles { get; set; }
        private int _periodTSI { get; set; }
        private int _periodSignalLine { get; set; }
        private decimal _valueBuy { get; set; }
        private decimal _valueSell { get; set; }
        public int[] periods { get; set; }

        public TrueStrengthIndex(int quantityCandles, int periodTSI, int periodSignalLine, decimal valueBuy, decimal valueSell)
        {
            _periodTSI = periodTSI;
            _periodSignalLine = periodSignalLine;
            _quantityCandles = quantityCandles;
            _valueBuy = valueBuy;
            _valueSell = valueSell;
        }

        public TrueStrengthIndex()
        {

        }

        public TrueStrengthIndex(int quantityCandles, int periodTSI, int periodSignalLine)
        {
            _periodTSI = periodTSI;
            _periodSignalLine = periodSignalLine;
            _quantityCandles = quantityCandles;
        }

        public TSIValues GetTSI(List<decimal> prices)
        {
            try
            {
                List<decimal> momentum = new List<decimal>();
                List<decimal> absMomentum = new List<decimal>();

                List<decimal> tsi = new List<decimal>();

                for (int i = 0; i < prices.Count - 1; i++)
                {
                    momentum.Add(prices[i + 1] - prices[i]);
                    absMomentum.Add(Math.Abs(prices[i + 1] - prices[i]));
                }

                var firstSmoothed = ExponentialMovingAverage.GetExponentialMovingAverages(momentum, _quantityCandles);
                var secondSmoothed = ExponentialMovingAverage.GetExponentialMovingAverages(firstSmoothed, _periodTSI);

                var firstAbsSmoothed = ExponentialMovingAverage.GetExponentialMovingAverages(absMomentum, _quantityCandles);
                var secondAbsSmoothed = ExponentialMovingAverage.GetExponentialMovingAverages(firstAbsSmoothed, _periodTSI);

                for (int i = 0; i < secondSmoothed.Count; i++)
                {
                    tsi.Add(secondSmoothed[i] / secondAbsSmoothed[i]);
                }

                return new TSIValues()
                {
                    TSI = tsi,
                    SignalLine = ExponentialMovingAverage.GetExponentialMovingAverages(tsi, _periodSignalLine)
                };
            }
            catch { Console.WriteLine("Ошибка в методе GetTSI"); throw new Exception("Ошибка в методе GetTSI"); }
        }
        public TSIValues GetTSI(List<decimal> prices, int quantityCandles, int periodTSI, int periodSignalLine)
        {
            try
            {
                List<decimal> momentum = new List<decimal>();
                List<decimal> absMomentum = new List<decimal>();

                List<decimal> tsi = new List<decimal>();

                for (int i = 0; i < prices.Count - 1; i++)
                {
                    momentum.Add(prices[i + 1] - prices[i]);
                    absMomentum.Add(Math.Abs(prices[i + 1] - prices[i]));
                }

                var firstSmoothed = ExponentialMovingAverage.GetExponentialMovingAverages(momentum, quantityCandles);
                var secondSmoothed = ExponentialMovingAverage.GetExponentialMovingAverages(firstSmoothed, periodTSI);

                var firstAbsSmoothed = ExponentialMovingAverage.GetExponentialMovingAverages(absMomentum, quantityCandles);
                var secondAbsSmoothed = ExponentialMovingAverage.GetExponentialMovingAverages(firstAbsSmoothed, periodTSI);

                for (int i = 0; i < secondSmoothed.Count; i++)
                {
                    tsi.Add(secondSmoothed[i] / secondAbsSmoothed[i]);
                }

                return new TSIValues()
                {
                    TSI = tsi,
                    SignalLine = ExponentialMovingAverage.GetExponentialMovingAverages(tsi, periodSignalLine)
                };
            }
            catch { Console.WriteLine("Ошибка в методе GetTSI"); throw new Exception("Ошибка в методе GetTSI"); }
        }

        public bool BuySignal(IEnumerable<decimal> prices)
        {
            var pricesClose = prices.ToList();

            TSIValues tsi = GetTSI(pricesClose);
            if(tsi.TSI.SkipLast(1).Last() > 0.15m && tsi.SignalLine.SkipLast(1).Last() > 0.1m)
            {
                return true;
            }
            return false;
        }
        public bool BuySignalCross(IEnumerable<decimal> prices)
        {
            throw new NotImplementedException();
        }
        public bool SellSignal(IEnumerable<decimal> prices)
        {
            throw new NotImplementedException();
        }
        public bool SellSignalCross(IEnumerable<decimal> prices)
        {
            throw new NotImplementedException();
        }

        public bool IsBuy(List<Candlestick> candlesticks)
        {
            TSIValues tsi = GetTSI(candlesticks.Select(x => x.Close).ToList());

            if(tsi.TSI.Last() < _valueBuy)
            {
                return true;
            }

            return false;
        }
        public bool IsSell(List<Candlestick> candlesticks)
        {
            TSIValues tsi = GetTSI(candlesticks.Select(x => x.Close).ToList());

            if (tsi.TSI.Last() > _valueSell)
            {
                return true;
            }

            return false;
        }

        public bool IsBuy(List<Candlestick> candlesticks, decimal value, params int[] periods)
        {
            TSIValues tsi = GetTSI(candlesticks.Select(x => x.Close).ToList(), periods[0], periods[1], 10);

            if (tsi.TSI.Last() < -value)
            {
                return true;
            }

            return false;
        }
        public bool IsSell(List<Candlestick> candlesticks, decimal value, params int[] periods)
        {
            TSIValues tsi = GetTSI(candlesticks.Select(x => x.Close).ToList(), periods[0], periods[1], 10);

            if (tsi.TSI.Last() > value)
            {
                return true;
            }

            return false;
        }
    }

    public class TSIValues
    {
        public List<decimal> TSI { get; set; }
        public List<decimal> SignalLine { get; set; }
    }
}
