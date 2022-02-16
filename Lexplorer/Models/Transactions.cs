using Newtonsoft.Json;
using Lexplorer.Helpers;

namespace Lexplorer.Models
{
    public class TransactionProxy
    {
        public string accountUpdateCount { get; set; }
        public string addCount { get; set; }
        public string ammUpdateCount { get; set; }
        public string depositCount { get; set; }
        public string nftDataCount { get; set; }
        public string nftMintCount { get; set; }
        public string orderbookTradeCount { get; set; }
        public string removeCount { get; set; }
        public string signatureVerificationCount { get; set; }
        public string swapCount { get; set; }
        public string swapNFTCount { get; set; }
        public string tradeNFTCount { get; set; }
        public string transactionCount { get; set; }
        public string transferCount { get; set; }
        public string transferNFTCount { get; set; }
        public string withdrawalCount { get; set; }
        public string withdrawalNFTCount { get; set; }
    }

    public class Account
    {
        public string address { get; set; }
        public string id { get; set; }
        [JsonProperty("__typename")]
        public string typeName { get; set; }
    }

    public class TransactionBlock
    {
        public string accountUpdateCount { get; set; }
        public string addCount { get; set; }
        public string ammUpdateCount { get; set; }
        public string blockHash { get; set; }
        public string depositCount { get; set; }
        public string id { get; set; }
        public string nftDataCount { get; set; }
        public string nftMintCount { get; set; }
        public string orderbookTradeCount { get; set; }
        public string removeCount { get; set; }
        public string signatureVerificationCount { get; set; }
        public string swapCount { get; set; }
        public string swapNFTCount { get; set; }
        public string timestamp { get; set; }
        public string tradeNFTCount { get; set; }
        public string transactionCount { get; set; }
        public string transferCount { get; set; }
        public string transferNFTCount { get; set; }
        public string withdrawalCount { get; set; }
        public string withdrawalNFTCount { get; set; }
        public string data { get; set; }
    }

    public class Token0
    {
        public string symbol { get; set; }
    }

    public class Token1
    {
        public string symbol { get; set; }
    }

    public class Pair
    {
        public string id { get; set; }
        public Token0 token0 { get; set; }
        public Token1 token1 { get; set; }
    }

    public class Token
    {
        public string address { get; set; }
        public int decimals { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
    }

    public class Balance
    {
        public string balance { get; set; }
        public string id { get; set; }
        public Token token { get; set; }
    }

    public class Pool : Account
    {
        public List<Balance> balances { get; set; }
    }

    public class TokenA
    {
        public string address { get; set; }
        public int decimals { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
    }

    public class TokenB
    {
        public string address { get; set; }
        public int decimals { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
    }

    public class FeeToken
    {
        public string address { get; set; }
        public int decimals { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
    }

    public class User : Account
    {
        public string publicKey { get; set; }
    }

    public class Transaction
    {
        [JsonProperty(PropertyName = "__typename")]
        public string typeName { get; set; }
        public Account account { get; set; }
        public TransactionBlock block { get; set; }
        public string data { get; set; }
        public string feeA { get; set; }
        public string feeB { get; set; }
        public string fillBA { get; set; }
        public string fillSA { get; set; }
        public string fillSB { get; set; }
        public string fillBB { get; set; }
        public bool fillAmountBorSA { get; set; }
        public bool fillAmountBorSB { get; set; }
        public string id { get; set; }
        public string internalID { get; set; }
        public Pair pair { get; set; }
        public Pool pool { get; set; }
        public string protocolFeeA { get; set; }
        public string protocolFeeB { get; set; }
        public TokenA tokenA { get; set; }
        public string tokenAPrice { get; set; }
        public TokenB tokenB { get; set; }
        public string tokenBPrice { get; set; }
        public string amount { get; set; }
        public string fee { get; set; }
        public FeeToken feeToken { get; set; }
        public Account fromAccount { get; set; }
        public Account toAccount { get; set; }
        public Token token { get; set; }
        public int? nonce { get; set; }
        public User user { get; set; }

        public Account accountA { get; set; }
        public Account accountB { get; set; }

        public string verifiedAt
        {
            get
            {
                if (block == null) return string.Empty;

                return TimestampToUTCConverter.Convert(block.timestamp);
            }
        }
    }

    public class TransactionsData
    {
        public TransactionProxy proxy { get; set; }
        public List<Transaction> transactions { get; set; }
    }

    public class Transactions
    {
        public TransactionsData data { get; set; }
    }

}
