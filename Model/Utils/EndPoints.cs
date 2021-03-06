namespace Model.Utils
{
    public static class EndPoints
    {
        #region General Endpoints
        public static readonly string TestConnectivity = "/api/v1/ping";
        public static readonly string CheckServerTime = "/api/v1/time";
        #endregion

        #region Market Data Endpoints
        public static readonly string OrderBook = "/api/v3/depth";
        public static readonly string AggregateTrades = "/api/v1/aggTrades";
        public static readonly string Candlesticks = "/api/v1/klines";
        public static readonly string TickerPriceChange24H = "/api/v1/ticker/24hr";
        public static readonly string AllPrices = "/api/v1/ticker/allPrices";
        public static readonly string OrderBookTicker = "/api/v1/ticker/allBookTickers";
        public static readonly string ExchangeInfo = "/api/v3/exchangeInfo";
        
        #endregion

        #region Account Endpoints
        public static readonly string QueryOCO = "/api/v3/orderList";
        public static readonly string OCOOrder = "/api/v3/order/oco";
        public static readonly string NewOrder = "/api/v3/order";
        public static readonly string NewOrderTest = "/api/v3/order/test";
        public static readonly string QueryOrder = "/api/v3/order";
        public static readonly string CancelOrder = "/api/v3/order";
        public static readonly string CurrentOpenOrders = "/api/v3/openOrders";
        public static readonly string AllOrders = "/api/v3/allOrders";
        public static readonly string AccountInformation = "/api/v3/account";
        public static readonly string TradeList = "/api/v3/myTrades";
        public static readonly string QueryOpenOCO = "/api/v3/openOrderList";
        #endregion

        #region Margin Account/Trade

        public static readonly string Transfer = "/sapi/v1/margin/transfer";
        public static readonly string Borrow = "/sapi/v1/margin/loan";
        public static readonly string Repay = "/sapi/v1/margin/repay";
        public static readonly string MaxBorrow = "/sapi/v1/margin/maxBorrowable";
        public static readonly string OrderMargin = "/sapi/v1/margin/order";
        public static readonly string MaxTransferOutAmount = "/sapi/v1/margin/maxTransferable";
        public static readonly string QueryRepayRecord = "/sapi/v1/margin/repay";
        public static readonly string QueryMarginAccountDetails = "/sapi/v1/margin/account";
        public static readonly string QueryTradeList = "/sapi/v1/margin/myTrades";
        public static readonly string MarginAsset = "/sapi/v1/margin/allAssets";

        #endregion
    }
}
