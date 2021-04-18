using Model.Models.Market;
using System.Collections.Generic;

namespace TechnicalAnalysis.Interfaces
{
    public interface IIndicator
    {
        public bool IsSell(List<Candlestick> candlesticks);
        public bool IsBuy(List<Candlestick> candlesticks);

    }
}
