using Model.Enums;

namespace BotBinanceBL.Stocks.Signals
{
    public class Signal
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal StopLimitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal StopLoss { get; set; }
        public OrderSide Side { get; set; }
    }
}