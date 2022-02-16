using Newtonsoft.Json;

namespace Lexplorer.Models
{
    public class OrderbookTradeBlock
    {
        public string id { get; set; }
        public string blockHash { get; set; }
        public string timestamp { get; set; }
        public string transactionCount { get; set; }
    }

    public class OrderbookTradeTokenA
    {
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public int decimals { get; set; }
        public string address { get; set; }
    }

    public class OrderbookTradeTokenB
    {
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public int decimals { get; set; }
        public string address { get; set; }
    }

    public class OrderbookTradeToken0
    {
        public string symbol { get; set; }
    }

    public class OrderbookTradeToken1
    {
        public string symbol { get; set; }
    }

    public class OrderbookTradePair
    {
        public string id { get; set; }
        public OrderbookTradeToken0 token0 { get; set; }
        public OrderbookTradeToken1 token1 { get; set; }
    }

    public class OrderbookTradeTransaction
    {
        public string id { get; set; }
        public OrderbookTradeBlock block { get; set; }
        public string data { get; set; }
        public Account accountA { get; set; }
        public Account accountB { get; set; }
        public OrderbookTradeTokenA tokenA { get; set; }
        public OrderbookTradeTokenB tokenB { get; set; }
        public OrderbookTradePair pair { get; set; }
        public string tokenAPrice { get; set; }
        public string tokenBPrice { get; set; }
        public string fillSA { get; set; }
        public string fillSB { get; set; }
        public string fillBA { get; set; }
        public string fillBB { get; set; }
        public bool fillAmountBorSA { get; set; }
        public bool fillAmountBorSB { get; set; }
        public string feeA { get; set; }
        public string feeB { get; set; }

        [JsonProperty(PropertyName = "__typename")]
        public string typeName { get; set; }
    }

    public class OrderbookTradeData
    {
        public OrderbookTradeTransaction transaction { get; set; }
    }

    public class OrderbookTrade
    {
        public OrderbookTradeData data { get; set; }
    }


}
