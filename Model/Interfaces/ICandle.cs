using Model.Enums;
using Model.Models.Market;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface ICandle
    {
        Task<IEnumerable<Candlestick>> GetCandleSticks(string symbol, TimeInterval interval, DateTime? startTime = null, DateTime? endTime = null, int limit = 500);
    }
}
