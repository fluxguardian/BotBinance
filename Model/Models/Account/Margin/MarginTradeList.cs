using Newtonsoft.Json;

namespace Model.Models.Account.Margin
{
    public class MarginTradeList
    {
        [JsonProperty("commission")]
        public decimal Commission { get; set; }

        [JsonProperty("commissionAsset")]
        public string CommissionAsset { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("isBestMatch")]
        public bool IsBestMatch { get; set; }

        [JsonProperty("isBuyer")]
        public bool IsBuyer { get; set; }

        [JsonProperty("isMaker")]
        public bool IsMaker { get; set; }

        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("isIsolated")]
        public bool IsIsolated { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }
    }
}
