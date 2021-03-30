using Newtonsoft.Json;
using System.Collections.Generic;

namespace Model.Models.Account.Spot
{
    public class OpenOCOOrder
    {
        [JsonProperty("orderListId")]
        public long OrderListId { get; set; }
        
        [JsonProperty("contingencyType")]
        public string ContingencyType { get; set; }

        [JsonProperty("listStatusType")]
        public string ListStatusType { get; set; }
        
        [JsonProperty("listOrderStatus")]
        public string ListOrderStatus { get; set; }
        
        [JsonProperty("listClientOrderId")]
        public string ListClientOrderId { get; set; }
        
        [JsonProperty("transactionTime")]
        public long TransactionTime { get; set; }
        
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        
        [JsonProperty("orders")]
        public List<QueryOrder> Orders { get; set; }
    }
}
