using Newtonsoft.Json;

namespace Lexplorer.Models
{
    public class SwapBlock
    {
        public string blockHash { get; set; }
        public string id { get; set; }
        public string timestamp { get; set; }
        public string transactionCount { get; set; }
    }

    public class SwapPair
    {
        public string id { get; set; }
        public Token token0 { get; set; }
        public Token token1 { get; set; }
    }
    public class SwapTransaction
    {
        [JsonProperty(PropertyName = "__typename")]
        public string typeName { get; set; }
        public Account account { get; set; }
        public SwapBlock block { get; set; }
        public string data { get; set; }
        public string feeA { get; set; }
        public string feeB { get; set; }
        public string fillBA { get; set; }
        public string fillSA { get; set; }
        public string fillSB { get; set; }
        public string id { get; set; }
        public SwapPair pair { get; set; }
        public Pool pool { get; set; }
        public string protocolFeeA { get; set; }
        public string protocolFeeB { get; set; }
        public Token tokenA { get; set; }
        public string tokenAPrice { get; set; }
        public Token tokenB { get; set; }
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
