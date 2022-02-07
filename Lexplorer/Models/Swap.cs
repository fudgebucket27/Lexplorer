using Newtonsoft.Json;

namespace Lexplorer.Models
{
    public class SwapAccount
    {
        public string address { get; set; }
        public string id { get; set; }
    }

    public class SwapBlock
    {
        public string blockHash { get; set; }
        public string id { get; set; }
        public string timestamp { get; set; }
        public string transactionCount { get; set; }
    }

    public class SwapToken0
    {
        public string symbol { get; set; }
    }

    public class SwapToken1
    {
        public string symbol { get; set; }
    }

    public class SwapPair
    {
        public string id { get; set; }
        public SwapToken0 token0 { get; set; }
        public SwapToken1 token1 { get; set; }
    }

    public class SwapToken
    {
        public string address { get; set; }
        public int decimals { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
    }

    public class SwapBalance
    {
        public string balance { get; set; }
        public string id { get; set; }
        public Token token { get; set; }
    }

    public class SwapPool
    {
        public string address { get; set; }
        public List<Balance> balances { get; set; }
        public string id { get; set; }
    }
    public class SwapTokenA
    {
        public string address { get; set; }
        public int decimals { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
    }

    public class SwapTokenB
    {
        public string address { get; set; }
        public int decimals { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
    }

    public class SwapTransaction
    {
        [JsonProperty(PropertyName = "__typename")]
        public string typeName { get; set; }
        public SwapAccount account { get; set; }
        public SwapBlock block { get; set; }
        public string data { get; set; }
        public string feeA { get; set; }
        public string feeB { get; set; }
        public string fillBA { get; set; }
        public string fillSA { get; set; }
        public string fillSB { get; set; }
        public string id { get; set; }
        public SwapPair pair { get; set; }
        public SwapPool pool { get; set; }
        public string protocolFeeA { get; set; }
        public string protocolFeeB { get; set; }
        public SwapTokenA tokenA { get; set; }
        public string tokenAPrice { get; set; }
        public SwapTokenB tokenB { get; set; }
        public string tokenBPrice { get; set; }
    }

    public class SwapData
    {
        public SwapTransaction transaction { get; set; }
    }

    public class Swap
    {
        public SwapData data { get; set; }
    }


}
