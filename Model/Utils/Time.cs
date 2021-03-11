using Model.Models.Market;
using System;

namespace Model.Utils
{
    public static class Time
    {
        public static int GetTimeSleepMilliseconds(this Candlestick candle)
        {
            var closeTime = DateTimeOffset.FromUnixTimeMilliseconds(candle.CloseTime).LocalDateTime;

            return (int)(closeTime - DateTime.Now).TotalMilliseconds;
        }
    }
}
