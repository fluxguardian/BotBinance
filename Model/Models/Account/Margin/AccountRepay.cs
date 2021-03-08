using Newtonsoft.Json;

namespace Model.Models.Account.Margin
{
    public class AccountRepay
    {
        [JsonProperty("tranId")]
        public long TransactionId { get; set; }
    }
}
