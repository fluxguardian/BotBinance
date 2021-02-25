using Model.TradingRules;
using System;
using System.Globalization;
using System.Linq;

namespace Strategy.TradeSettings
{
    public class Normalization
    {
        private Symbol Asset { get; set; }
        public Round Round { get; set; }

        public Normalization(Symbol asset)
        {
            Asset = asset;
            Round = GetRoundPriceQuantity(Asset);
        }

        public decimal NormalizeSell(decimal balance)
        {
            return Math.Floor(balance * Convert.ToDecimal(Math.Pow(10, Round.RoundQuantity))) / Convert.ToDecimal(Math.Pow(10, Round.RoundQuantity));
        }

        public decimal NormalizeBuy(decimal balance)
        {
            return Math.Floor(balance * Convert.ToDecimal(Math.Pow(10, Round.RoundPrice))) / Convert.ToDecimal(Math.Pow(10, Round.RoundPrice));
        }

        private Round GetRoundPriceQuantity(Symbol asset)
        {
            var result = asset.Filters;

            var roundQuantity = result
                .Select(x => x.MinQty).ToList()[2].ToString(CultureInfo.InvariantCulture)
                .Split('.').Last().IndexOf('1') + 1;

            var roundPrice = result.First().MinPrice.ToString(CultureInfo.InvariantCulture)
                .Split('.').Last().IndexOf('1') + 1;

            return new Round()
            {
                RoundPrice = roundPrice,
                RoundQuantity = roundQuantity
            };
        }
    }

    public class Round
    {
        public int RoundPrice { get; set; }
        public int RoundQuantity { get; set; }
    }
}
