using Newtonsoft.Json;

namespace Model.Models.Account
{
    public class MaxTransferOutAmount
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}
