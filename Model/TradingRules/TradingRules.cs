﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace Model.TradingRules
{
    public class TradingRules
    {
        [JsonProperty("timezone")]
        public string Timezone { get; set; }
        [JsonProperty("serverTime")]
        public long ServerTime { get; set; }
        [JsonProperty("rateLimits")]
        public IEnumerable<RateLimit> RateLimits { get; set; }
        [JsonProperty("symbols")]
        public IEnumerable<Symbol> Symbols { get; set; }
    }
}
