using Newtonsoft.Json;

namespace Model.Models.Account
{
    public class AccountRepay
    {
        [JsonProperty("tranId")]
        public long TransactionId { get; set; }
    }
}
