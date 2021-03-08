using Newtonsoft.Json;

namespace Model.Models.Account.Margin
{
    public class AccountTransfer
    {
        [JsonProperty("tranId")]
        public long TransactionId { get; set; }
    }
}
