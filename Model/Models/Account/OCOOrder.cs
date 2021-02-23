using Newtonsoft.Json;
using System.Collections.Generic;

namespace Model.Models.Account
{
    public class OCOOrder
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

        [JsonProperty("orderReports")]
        public List<OrderReport> OrderReports { get; set; }
    }

    public class QueryOrder
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }    
    }

    public class OrderReport
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("orderListId")]
        public long OrderListId { get; set; }

        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }

        [JsonProperty("transactTime")]
        public long TransactTime { get; set; }
        
        [JsonProperty("price")]
        public decimal Price { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("timeInForce")]
        public string TimeInForce { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("side")]
        public string Side { get; set; }
        
        [JsonProperty("stopPrice")]
        public decimal StopPrice { get; set; }
    }

}
