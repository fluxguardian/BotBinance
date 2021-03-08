using BotBinanceBL.Stocks.Interfaces;
using Model.Enums;
using Model.Models.Account.Margin;
using Model.Models.Account.Spot;
using Model.Models.Market;
using Model.TradingRules;
using Model.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BotBinanceBL.Requests
{
    public class BinanceRequest : IRequest
    {
        private HttpUtilities _httpUtilities { get; set; }

        public BinanceRequest(HttpUtilities httpUtilities)
        {
            _httpUtilities = httpUtilities;
        }

        public async Task<T> CallAsync<T>(ApiMethod method, string endpoint, bool isSigned = false, string parameters = null)
        {
            string finalEndpoint = _httpUtilities.GetFinalPoint(endpoint, isSigned, ref parameters);

            HttpResponseMessage response = await _httpUtilities.GetHttpResponse(method, finalEndpoint);

            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<T>(result);
            }

            if (response.StatusCode == HttpStatusCode.GatewayTimeout)
            {
                throw new Exception("Api Request Timeout.");
            }

            throw new Exception("Error...");
        }

        #region Margin

        public async Task<AccountTransfer> Transfer(string asset, decimal amount, TransferType transferType)
        {
            if (string.IsNullOrWhiteSpace(asset))
            {
                throw new ArgumentException("asset cannot be emtpy");
            }

            string args = $"asset={asset}&amount={amount.ToString(CultureInfo.InvariantCulture)}&type={transferType.GetDescription()}&recvWindow=5000";

            return await CallAsync<AccountTransfer>(ApiMethod.POST, EndPoints.Transfer, true, args);
        }
        public async Task<AccountBorrow> Borrow(string asset, decimal amount, string isIsolated = "FALSE")
        {
            if (string.IsNullOrWhiteSpace(asset))
            {
                throw new ArgumentException("asset cannot be emtpy");
            }

            string args = $"asset={asset}&amount={amount.ToString(CultureInfo.InvariantCulture)}&isIsolated={isIsolated}&recvWindow=5000";

            return await CallAsync<AccountBorrow>(ApiMethod.POST, EndPoints.Borrow, true, args);
        }
        public async Task<AccountRepay> Repay(string asset, decimal amount, string isIsolated = "FALSE")
        {
            if (string.IsNullOrWhiteSpace(asset))
            {
                throw new ArgumentException("asset cannot be emtpy");
            }

            string args = $"asset={asset}&amount={amount.ToString(CultureInfo.InvariantCulture)}&isIsolated={isIsolated}&recvWindow=5000";

            return await CallAsync<AccountRepay>(ApiMethod.POST, EndPoints.Repay, true, args);
        }
        public async Task<MaxBorrow> MaxBorrow(string asset)
        {
            if (string.IsNullOrWhiteSpace(asset))
            {
                throw new ArgumentException("asset cannot be emtpy");
            }

            string args = $"asset={asset}&recvWindow=5000";

            return await CallAsync<MaxBorrow>(ApiMethod.GET, EndPoints.MaxBorrow, true, args);
        }
        public async Task<OrderMargin> MarketOrderQuantityMargin(string symbol, decimal quantity, OrderSide side, string isIsolated = "FALSE")
        {
            var args = $"symbol={symbol}&quantity={quantity.ToString(CultureInfo.InvariantCulture)}" +
                $"&type={OrderType.MARKET}&side={side}" +
                $"&recvWindow=10000" +
                $"&isIsolated={isIsolated}";

            return await CallAsync<OrderMargin>(ApiMethod.POST, EndPoints.OrderMargin, true, args);
        }
        public async Task<OrderMargin> MarketOrderQuoteMargin(string symbol, decimal quoteOrderQty, OrderSide side, string isIsolated = "FALSE")
        {
            var args = $"symbol={symbol}&quoteOrderQty={quoteOrderQty.ToString(CultureInfo.InvariantCulture)}" +
                $"&type={OrderType.MARKET}&side={side}" +
                $"&recvWindow=10000" +
                $"&isIsolated={isIsolated}";

            return await CallAsync<OrderMargin>(ApiMethod.POST, EndPoints.OrderMargin, true, args);
        }
        public async Task<MaxTransferOutAmount> MaxTransferOutAmount(string asset, string isIsolated = "FALSE")
        {
            if (string.IsNullOrWhiteSpace(asset))
            {
                throw new ArgumentException("asset cannot be emtpy");
            }

            string args = $"asset={asset}&recvWindow=5000&isIsolated={isIsolated}";

            return await CallAsync<MaxTransferOutAmount>(ApiMethod.GET, EndPoints.MaxTransferOutAmount, true, args);
        }
        public async Task<OrderMargin> OrderMarginStopLoss(string symbol, decimal quantity, decimal price, decimal stopPrice, OrderSide orderSide, 
            OrderType orderType, TimeInForce timeInForce = TimeInForce.GTC)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be emtpy");
            }

            var args = $"symbol={symbol}" +
                $"&type={orderType}&side={orderSide}" +
                $"&recvWindow=10000" +
                $"&price={price.ToString(CultureInfo.InvariantCulture)}" +
                $"&stopPrice={stopPrice.ToString(CultureInfo.InvariantCulture)}" +
                $"&quantity={quantity.ToString(CultureInfo.InvariantCulture)}" +
                $"&timeInForce={timeInForce}";

            return await CallAsync<OrderMargin>(ApiMethod.POST, EndPoints.OrderMargin, true, args);
        }
        public async Task<CanceledOrderMargin> CancelOrderMargin(string symbol, string origClientOrderId)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be emtpy");
            }

            var args = $"symbol={symbol}&origClientOrderId={origClientOrderId}";

            return await CallAsync<CanceledOrderMargin>(ApiMethod.DELETE, EndPoints.OrderMargin, true, args);
        }
        public async Task<QueryMarginOrder> QueryMarginOrder(string symbol, string origClientOrderId, string isIsolated = "FALSE")
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be emtpy");
            }

            var args = $"symbol={symbol}&origClientOrderId={origClientOrderId}&isIsolated={isIsolated}";

            return await CallAsync<QueryMarginOrder>(ApiMethod.GET, EndPoints.OrderMargin, true, args);
        }
        public async Task<QueryRepay> QueryRepayRecord(string asset, DateTime startTime, DateTime? endTime = null, long size = 10)
        {
            if (string.IsNullOrWhiteSpace(asset))
            {
                throw new ArgumentException("asset cannot be empty");
            }
            else if(startTime < endTime)
            {
                throw new ArgumentException("startTime cannot be less than endTime");
            }
            else if(startTime == null)
            {
                throw new ArgumentException("startTime cannot be null");
            }

            var args = $"asset={asset}" +
                $"&startTime={startTime.GetUnixTimeStamp()}" +
                (endTime.HasValue ? $"&endTime={endTime.Value.GetUnixTimeStamp()}" : "") +
                $"&size={size}&recvWindow=5000";

            return await CallAsync<QueryRepay>(ApiMethod.GET, EndPoints.QueryRepayRecord, true, args);
        }
        public async Task<QueryMarginAccountDetails> MarginAccountDetails()
        {
            return await CallAsync<QueryMarginAccountDetails>(ApiMethod.GET, EndPoints.QueryMarginAccountDetails, true);
        }
        public async Task<IEnumerable<MarginTradeList>> MarginTradeLists(string symbol, DateTime? startTime = null, DateTime? endTime = null, 
            int? fromId = null, string isIsolated = "FALSE", int limit = 500)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty");
            }
            else if (startTime < endTime)
            {
                throw new ArgumentException("startTime cannot be less than endTime");
            }

            var args = $"symbol={symbol}" +
                (startTime.HasValue ? $"&startTime={startTime.Value.GetUnixTimeStamp()}" : "") +
                (endTime.HasValue ? $"&endTime={endTime.Value.GetUnixTimeStamp()}" : "") +
                (fromId.HasValue ? $"fromId={fromId}" : "") +
                $"&isIsolated={isIsolated}" +
                $"&limit={limit}" +
                $"&recvWindow=5000";

            return await CallAsync<IEnumerable<MarginTradeList>>(ApiMethod.GET, EndPoints.QueryTradeList, true, args);
        }
        public async Task<IEnumerable<MarginAsset>> GetMarginAssets()
        {
            return await CallAsync<IEnumerable<MarginAsset>>(ApiMethod.GET, EndPoints.MarginAsset);
        }

        #endregion


        #region Spot
        public async Task<NewOrder> OrderQuantityMarket(string symbol, decimal quantity, OrderSide side)
        {
            string newquantity = quantity.ToString().Replace(',', '.');
            string param = $"symbol={symbol}&quantity={newquantity}&side={side}&type={OrderType.MARKET}&recvWindow=10000";

            return await CallAsync<NewOrder>(ApiMethod.POST, EndPoints.NewOrder, true, param);
        }
        public async Task<NewOrder> OrderQuoteMarket(string symbol, decimal quoteOrderQty, OrderSide side)
        {
            string newquoteOrderQty = quoteOrderQty.ToString().Replace(',', '.');
            string param = $"symbol={symbol}&quoteOrderQty={newquoteOrderQty}&side={side}&type={OrderType.MARKET}&recvWindow=10000";

            return await CallAsync<NewOrder>(ApiMethod.POST, EndPoints.NewOrder, true, param);
        }
        public async Task<QueryOCO> CancelOCO(string symbol, string listClientOrderId)
        {
            return await CallAsync<QueryOCO>(ApiMethod.DELETE, EndPoints.QueryOCO, true, $"symbol={symbol}&listClientOrderId={listClientOrderId}&recvWindow=5000");
        }
        public async Task<QueryOCO> QueryOCO(long orderListId)
        {
            return await CallAsync<QueryOCO>(ApiMethod.GET, EndPoints.QueryOCO, true, $"orderListId={orderListId}&recvWindow=5000");
        }
        public async Task<CanceledOrder> CancelOrder(string symbol, long? orderId = null, string origClientOrderId = null, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }

            var args = $"symbol={symbol.ToUpper()}&recvWindow={recvWindow}";

            if (orderId.HasValue)
            {
                args += $"&orderId={orderId.Value}";
            }
            else if (string.IsNullOrWhiteSpace(origClientOrderId))
            {
                args += $"&origClientOrderId={origClientOrderId}";
            }
            else
            {
                throw new ArgumentException("Either orderId or origClientOrderId must be sent.");
            }

            return await CallAsync<CanceledOrder>(ApiMethod.DELETE, EndPoints.CancelOrder, true, args);
        }
        public async Task<Order> GetOrder(string symbol, long? orderId = null, string origClientOrderId = null, long recvWindow = 5000)
        {
            var args = $"symbol={symbol.ToUpper()}&recvWindow={recvWindow}";

            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }

            if (orderId.HasValue)
            {
                args += $"&orderId={orderId.Value}";
            }
            else if (!string.IsNullOrWhiteSpace(origClientOrderId))
            {
                args += $"&origClientOrderId={origClientOrderId}";
            }
            else
            {
                throw new ArgumentException("Either orderId or origClientOrderId must be sent.");
            }

            return await CallAsync<Order>(ApiMethod.GET, EndPoints.QueryOrder, true, args);
        }
        public async Task<NewOrder> PostNewOrder(string symbol, decimal quantity, decimal price, OrderSide side, OrderType orderType,
            TimeInForce timeInForce, decimal icebergQty = 0, long recvWindow = 5000)
        {
            string newPrice = price.ToString();
            newPrice = newPrice.Replace(',', '.');

            string newQuantity = quantity.ToString();
            newQuantity = newQuantity.Replace(',', '.');

            string param = $"symbol={symbol}&side={side}&type={orderType}&timeInForce={timeInForce}&recvWindow=5000&quantity={newQuantity}&price={newPrice}";

            return await CallAsync<NewOrder>(ApiMethod.POST, EndPoints.NewOrder, true, param);
        }
        public async Task<OCOOrder> PostOCOOrder(string symbol, decimal quantity, decimal price, decimal stopPrice, decimal stopLimitPrice,
            OrderSide side, TimeInForce timeInForce = TimeInForce.GTC, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }

            string param = $"symbol={symbol}" +
                $"&quantity={quantity.ToString().Replace(',', '.')}" +
                $"&side={side}" +
                $"&price={price.ToString().Replace(',', '.')}" +
                $"&stopPrice={stopPrice.ToString().Replace(',', '.')}" +
                $"&stopLimitPrice={stopLimitPrice.ToString().Replace(',', '.')}" +
                $"&stopLimitTimeInForce={timeInForce}";

            return await CallAsync<OCOOrder>(ApiMethod.POST, EndPoints.OCOOrder, true, param);
        }
        public async Task<IEnumerable<Order>> GetAllOrders(string symbol, long? orderId = null, int limit = 500, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }

            return await CallAsync<IEnumerable<Order>>(ApiMethod.GET, EndPoints.AllOrders, true,
                $"symbol={symbol}&limit={limit}&recvWindow={recvWindow}" + (orderId.HasValue ? $"&orderId={orderId.Value}" : ""));
        }
        public async Task<IEnumerable<Order>> GetCurrentOpenOrders(string symbol, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }

            return await CallAsync<IEnumerable<Order>>(ApiMethod.GET, EndPoints.CurrentOpenOrders, true, $"symbol={symbol}&recvWindow={recvWindow}");
        }
        public async Task<IEnumerable<Candlestick>> GetCandleSticks(string symbol, TimeInterval interval, DateTime? startTime = null, DateTime? endTime = null, int limit = 500)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }

            var args = $"symbol={symbol}&interval={interval.GetDescription()}"
                + (startTime.HasValue ? $"&startTime={startTime.Value.GetUnixTimeStamp()}" : "")
                + (endTime.HasValue ? $"&endTime={endTime.Value.GetUnixTimeStamp()}" : "")
                + $"&limit={limit}";

            var result = await CallAsync<dynamic>(ApiMethod.GET, EndPoints.Candlesticks, false, args);

            return ExtensionMethods.GetParsedCandlestick(result);
        }
        public async Task<AccountInfo> GetAccountInfoAsync(long recvWindow = 5000)
        {
            return await CallAsync<AccountInfo>(ApiMethod.GET, EndPoints.AccountInformation, true, $"recvWindow={recvWindow}");
        }
        public async Task<IEnumerable<Trade>> GetTradeList(string symbol, int limit = 500, long recvWindow = 5000)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol cannot be empty. ", "symbol");
            }

            return await CallAsync<IEnumerable<Trade>>(ApiMethod.GET, EndPoints.TradeList, true, $"symbol={symbol}&limit={limit}&recvWindow={recvWindow}");
        }
        public async Task<TradingRules> GetExchangeInformationAsync()
        {
            return await CallAsync<TradingRules>(ApiMethod.GET, EndPoints.ExchangeInfo);
        }

        #endregion
    }
}
