using Newtonsoft.Json;

namespace Model.Models.Account.Margin
{
    public class CanceledOrderMargin
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("orderId")]
        public int OrderId { get; set; }

        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("executedQty")]
        public string ExecutedQty { get; set; }
    }
}
