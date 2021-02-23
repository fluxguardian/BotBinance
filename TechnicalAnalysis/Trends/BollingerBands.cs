using Model.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using TechnicalAnalysis;

namespace Trends.TechnicalAnalysis
{
    /// <summary>
    /// Индикатор - Полоса Боллинджера
    /// </summary>
    public class BollingerBands
    {
        /// <summary>
        /// Период расчета (по умолчанию равен 20)
        /// </summary>
        private int Period { get; set; }

        /// <summary>
        /// Отклонение от средней полосы Боллинджера (по умолчанию равен 2)
        /// </summary>
        private int QuantityDeviation { get; set; }

        /// <summary>
        /// Цены (закрытия, открытия) валюты
        /// </summary>
        private List<Candlestick> Prices { get; set; }

        public BollingerBands(List<Candlestick> prices, int quantityDev = 2, int period = 20)
        {
            Prices = prices;

            Period = period;
            QuantityDeviation = quantityDev;
        }

        /// <summary>
        /// Расчет одной (последней) полосы Боллинджера
        /// </summary>
        /// <param name="prices">Цены закрытия свеч</param>
        /// <param name="period">Период простой скользящей средней</param>
        /// <param name="stdDev">Количество смещений страндартного отклонения</param>
        /// <returns>Значение (нижняя, средняя, верхняя) полосы Боллинджера</returns>
        public Band GetBollingerBands()
        {
            var prices = Prices.TakeLast(Period).Select(x => x.Close).ToList();

            decimal middleLine = SimpleMovingAverage.GetSimpleMovingAverage(prices, Period);

            var sum = 0.0;
            prices.ForEach(x => sum += Math.Pow(Convert.ToDouble(x - middleLine), 2));

            decimal stdDev = Convert.ToDecimal(Math.Sqrt(sum / Period));

            decimal upLine = middleLine + (QuantityDeviation * stdDev);
            decimal downLine = middleLine - (QuantityDeviation * stdDev);

            return new Band() 
            { 
                UpLine = upLine, 
                MiddleLine = middleLine, 
                DownLine = downLine,
                Width = (upLine - downLine) / middleLine,
                BBB = (prices.Last() - downLine) / (upLine - downLine)
            };
        }

        /// <summary>
        /// Расчет полос Боллинджера
        /// </summary>
        /// <param name="prices">Цены закрытия свеч</param>
        /// <param name="period">Период простой скользящей средней</param>
        /// <param name="stdDev">Количество смещений страндартного отклонения</param>
        /// <returns>Список значений (нижняя, средняя, верхняя полоса) Боллинджера</returns>
        public List<Band> GetAllBollingerBands()
        {
            List<Band> lstBands = new List<Band>();
            List<decimal> currentPrices = new List<decimal>();

            var prices = Prices.Select(x => x.Close).ToList();

            for (int i = Period; i <= Prices.Count; i++)
            {
                currentPrices.AddRange(prices.Skip(i - Period).Take(Period).ToList());
                decimal middleLine = SimpleMovingAverage.GetSimpleMovingAverage(currentPrices, Period);

                var sum = 0.0;
                currentPrices.ForEach(x => sum += Math.Pow(Convert.ToDouble(x - middleLine), 2));

                decimal stdDev = Convert.ToDecimal(Math.Sqrt(sum / Period));

                decimal upLine = middleLine + (QuantityDeviation * stdDev);
                decimal downLine = middleLine - (QuantityDeviation * stdDev);

                lstBands.Add(new Band()
                {
                    DownLine = downLine,
                    MiddleLine = middleLine,
                    UpLine = upLine,
                    Width = (upLine - downLine) / middleLine, 
                    BBB = (currentPrices.Last() - downLine) / (upLine - downLine)
                });

                currentPrices = new List<decimal>();
            }
            return lstBands;
        }

        /// <summary>
        /// Определение флэта или тренда (восходящий, нисходящий)
        /// </summary>
        /// <returns>флэт или тренд</returns>
        public string GetTrend()
        {
            var lastBands = GetBollingerBands();

            string trend = lastBands.Width < 0.04m ? "Флэт" : "Тренд";

            if (trend.Equals("Тренд"))
            {
                trend = lastBands.BBB > 0.5m ? "Восходящий" : "Нисходящий";
            }

            return trend;
        }
    }

    /// <summary>
    /// Полоса
    /// </summary>
    public class Band
    {
        /// <summary>
        /// Верхняя линия
        /// </summary>
        public decimal UpLine { get; set; }

        /// <summary>
        /// Средняя линия
        /// </summary>
        public decimal MiddleLine { get; set; }

        /// <summary>
        /// Нижняя линия
        /// </summary>
        public decimal DownLine { get; set; }

        /// <summary>
        /// Ширина полосы
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// Положение цены закрытия относительно полос Боллинджера
        /// </summary>
        public decimal BBB { get; set; }
    }
}
