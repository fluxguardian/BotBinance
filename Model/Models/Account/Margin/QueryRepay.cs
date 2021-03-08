using Newtonsoft.Json;
using System.Collections.Generic;

namespace Model.Models.Account.Margin
{
    public class QueryRepay
    {
        [JsonProperty("rows")]
        public List<Row> Rows { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }

    public class Row
    {
        [JsonProperty("isolatedSymbol")]
        public string IsolatedSymbol { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("asset")]
        public string Asset { get; set; }

        [JsonProperty("interest")]
        public decimal Interest { get; set; }

        [JsonProperty("principal")]
        public decimal Principal { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("timestamp")]
        public long TimeStamp { get; set; }

        [JsonProperty("txId")]
        public long TxId { get; set; }
    }
}
