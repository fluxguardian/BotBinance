using Newtonsoft.Json;

namespace Model.Models.Account.Margin
{
    public class OrderMargin
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("orderId")]
        public int OrderId { get; set; }

        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }

        [JsonProperty("transactTime")]
        public long TransactTime { get; set; }

        [JsonProperty("isIsolated")]
        public bool IsIsolated { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("origQty")]
        public decimal OrigQty { get; set; }

        [JsonProperty("executedQty")]
        public decimal ExecutedQty { get; set; }

        [JsonProperty("cummulativeQuoteQty")]
        public decimal CummulativeQuoteQty { get; set; }

        [JsonProperty("timeInForce")]
        public string TimeInForce { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
