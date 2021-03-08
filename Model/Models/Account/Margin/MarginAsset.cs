using Newtonsoft.Json;

namespace Model.Models.Account.Margin
{
    public class MarginAsset
    {
        [JsonProperty("assetFullName")]
        public string AssetFullName { get; set; }

        [JsonProperty("assetName")]
        public string AssetName { get; set; }

        [JsonProperty("isBorrowable")]
        public bool IsBorrowable { get; set; }

        [JsonProperty("isMortgageable")]
        public bool IsMortgageable { get; set; }

        [JsonProperty("userMinBorrow")]
        public decimal UserMinBorrow { get; set; }

        [JsonProperty("userMinRepay")]
        public decimal UserMinRepay { get; set; }
    }
}
