using Newtonsoft.Json;

namespace Model.Models.Account.Margin
{
    public class QueryMarginOrder
    {
        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }

        [JsonProperty("cummulativeQuoteQty")]
        public decimal CummulativeQuoteQty { get; set; }

        [JsonProperty("executedQty")]
        public decimal ExecutedQty { get; set; }

        [JsonProperty("icebergQty")]
        public decimal IcebergQty { get; set; }

        [JsonProperty("isWorking")]
        public bool IsWorking { get; set; }

        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("origQty")]
        public decimal OrigQty { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("stopPrice")]
        public decimal StopPrice { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("isIsolated")]
        public bool IsIsolated { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("timeInForce")]
        public string TimeInForce { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("updateTime")]
        public long UpdateTime { get; set; }
    }
}
