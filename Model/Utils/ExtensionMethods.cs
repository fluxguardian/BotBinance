using Model.Models.Market;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Model.Utils
{
    public static class ExtensionMethods
    {
        public static string GetDescription(this Enum value)
        {
            return ((DescriptionAttribute)Attribute.GetCustomAttribute(
                value.GetType().GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Single(x => x.GetValue(null).Equals(value)),
                typeof(DescriptionAttribute)))?.Description ?? value.ToString();
        }

        public static string GetUnixTimeStamp(this DateTime baseDateTime)
        {
            var dtOffset = new DateTimeOffset(baseDateTime);
            return dtOffset.ToUnixTimeMilliseconds().ToString();
        }

        public static IEnumerable<Candlestick> GetParsedCandlestick(dynamic candlestickData)
        {
            var result = new List<Candlestick>();

            foreach (JToken item in ((JArray)candlestickData).ToList())
            {
                result.Add(new Candlestick()
                {
                    OpenTime = Int64.Parse(item[0].ToString()),
                    Open = Convert.ToDecimal(item[1]),
                    High = Convert.ToDecimal(item[2]),
                    Low = Convert.ToDecimal(item[3]),
                    Close = Convert.ToDecimal(item[4]),
                    Volume = Convert.ToDecimal(item[5]),
                    CloseTime = Int64.Parse(item[6].ToString()),
                    QuoteAssetVolume = Convert.ToDecimal(item[7]),
                    NumberOfTrades = int.Parse(item[8].ToString()),
                    TakerBuyBaseAssetVolume = Convert.ToDecimal(item[9]),
                    TakerBuyQuoteAssetVolume = Convert.ToDecimal(item[10])
                });
            }
            return result;
        }

        public static OrderBook GetParsedOrderBook(dynamic orderBookData)
        {
            var result = new OrderBook
            {
                LastUpdateId = orderBookData.lastUpdateId.Value
            };

            var bids = new List<OrderBookOffer>();
            var asks = new List<OrderBookOffer>();

            foreach (JToken item in ((JArray)orderBookData.bids).ToArray())
            {
                bids.Add(new OrderBookOffer() 
                { 
                    Price = decimal.Parse(item[0].ToString().Replace('.', ',')),
                    Quantity = decimal.Parse(item[1].ToString().Replace('.', ',')) 
                });
            }

            foreach (JToken item in ((JArray)orderBookData.asks).ToArray())
            {
                asks.Add(new OrderBookOffer() 
                { 
                    Price = decimal.Parse(item[0].ToString().Replace('.', ',')), 
                    Quantity = decimal.Parse(item[1].ToString().Replace('.', ',')) 
                });
            }

            result.Bids = bids;
            result.Asks = asks;

            return result;
        }
    }
}
