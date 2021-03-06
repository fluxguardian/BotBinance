using Newtonsoft.Json;

namespace Model.Models.Account.Spot
{
    public class CanceledOrder
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("orderId")]
        public long OrderId { get; set; }
        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }
        [JsonProperty("origClientOrderId ")]
        public string OrigClientOrderId { get; set; }
    }
}
