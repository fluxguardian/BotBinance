using Newtonsoft.Json;

namespace Model.Models.Account.Margin
{
    public class MaxTransferOutAmount
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}
