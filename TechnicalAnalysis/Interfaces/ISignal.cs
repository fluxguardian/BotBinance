using System.Collections.Generic;

namespace TechnicalAnalysis
{
    public interface ISignal
    {
        public bool BuySignalCross(IEnumerable<decimal> prices);
        public bool SellSignalCross(IEnumerable<decimal> prices);

        public bool BuySignal(IEnumerable<decimal> prices);
        public bool SellSignal(IEnumerable<decimal> prices);
    }
}
