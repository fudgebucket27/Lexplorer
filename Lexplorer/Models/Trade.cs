using Newtonsoft.Json;

namespace Lexplorer.Models
{
    public class TradeBlock
    {
        public string id { get; set; }
        public string blockHash { get; set; }
        public string timestamp { get; set; }
        public string transactionCount { get; set; }
    }

    public class TradeAccountA
    {
        public string id { get; set; }
        public string address { get; set; }
    }

    public class TradeAccountB
    {
        public string id { get; set; }
        public string address { get; set; }
    }

    public class TradeTokenA
    {
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public int decimals { get; set; }
        public string address { get; set; }
    }

    public class TradeTokenB
    {
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public int decimals { get; set; }
        public string address { get; set; }
    }

    public class TradeToken0
    {
        public string symbol { get; set; }
    }

    public class TradeToken1
    {
        public string symbol { get; set; }
    }

    public class TradePair
    {
        public string id { get; set; }
        public TradeToken0 token0 { get; set; }
        public TradeToken1 token1 { get; set; }
    }

    public class TradeTransaction
    {
        public string id { get; set; }
        public TradeBlock block { get; set; }
        public string data { get; set; }
        public TradeAccountA accountA { get; set; }
        public TradeAccountB accountB { get; set; }
        public TradeTokenA tokenA { get; set; }
        public TradeTokenB tokenB { get; set; }
        public TradePair pair { get; set; }
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

    public class TradeData
    {
        public TradeTransaction transaction { get; set; }
    }

    public class Trade
    {
        public TradeData data { get; set; }
    }


}
