using Newtonsoft.Json;

namespace Model.Models.Account.Margin
{
    public class AccountBorrow
    {
        [JsonProperty("tranId")]
        public long TransactionId { get; set; }
    }
}
