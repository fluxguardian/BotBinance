﻿using Newtonsoft.Json;

namespace Model.Models.Account
{
    public class AccountTransfer
    {
        [JsonProperty("tranId")]
        public long TransactionId { get; set; }
    }
}
