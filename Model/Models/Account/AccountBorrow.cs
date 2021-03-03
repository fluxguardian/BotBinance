using Newtonsoft.Json;

namespace Model.Models.Account
{
    public class AccountBorrow
    {
        [JsonProperty("tranId")]
        public long TransactionId { get; set; }
    }
}
