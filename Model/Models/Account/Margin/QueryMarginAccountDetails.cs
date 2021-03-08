using Newtonsoft.Json;
using System.Collections.Generic;

namespace Model.Models.Account.Margin
{
    public class QueryMarginAccountDetails
    {
        [JsonProperty("borrowEnabled")]
        public bool BorrowEnabled { get; set; }

        [JsonProperty("marginLevel")]
        public decimal MarginLevel { get; set; }

        [JsonProperty("totalAssetOfBtc")]
        public decimal TotalAssetOfBtc { get; set; }

        [JsonProperty("totalLiabilityOfBtc")]
        public decimal TotalLiabilityOfBtc { get; set; }

        [JsonProperty("totalNetAssetOfBtc")]
        public decimal TotalNetAssetOfBtc { get; set; }

        [JsonProperty("tradeEnabled")]
        public bool TradeEnabled { get; set; }

        [JsonProperty("transferEnabled")]
        public bool TransferEnabled { get; set; }

        [JsonProperty("userAssets")]
        public List<UserAsset> UserAssets { get; set; }
    }

    public class UserAsset
    {
        [JsonProperty("asset")]
        public string Asset { get; set; }

        [JsonProperty("borrowed")]
        public decimal Borrowed { get; set; }

        [JsonProperty("free")]
        public decimal Free { get; set; }

        [JsonProperty("interest")]
        public decimal Interest { get; set; }

        [JsonProperty("locked")]
        public decimal Locked { get; set; }

        [JsonProperty("netAsset")]
        public decimal NetAsset { get; set; }
    }
}
