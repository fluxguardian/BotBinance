using Model.Models.Market;
using System;
using System.Collections.Generic;
using System.Text;

namespace TechnicalAnalysis.Interfaces
{
    public interface IIndicator
    {
        public bool IsBuy(List<Candlestick> candlesticks);
        public bool IsBuy(List<Candlestick> candlesticks, decimal value, params int[] periods);
        public bool IsSell(List<Candlestick> candlesticks);
        public bool IsSell(List<Candlestick> candlesticks, decimal value, params int[] periods);
    }
}
