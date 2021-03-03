using Newtonsoft.Json;

namespace Model.Models.Account
{
    public class MaxBorrow
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("borrowLimit")]
        public long BorrowLimit { get; set; }
    }
}
