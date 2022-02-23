using Newtonsoft.Json;
using Lexplorer.Helpers;
using JsonSubTypes;

//source: https://thegraph.com/hosted-service/subgraph/loopring/loopring

namespace Lexplorer.Models
{
    public class BlockDetail
    {
        public Int64 accountUpdateCount { get; set; }
        public Int64 addCount { get; set; }
        public Int64 ammUpdateCount { get; set; }
        public string? blockHash { get; set; }
        public int blockSize { get; set; }
        public string? data { get; set; }
        public Int64 depositCount { get; set; }
        public Double gasLimit { get; set; }
        public Double gasPrice { get; set; }
        public Double height { get; set; }
        public string? id { get; set; }
        public Int64 nftDataCount { get; set; }
        public Int64 nftMintCount { get; set; }
        public Account? operatorAccount { get; set; }
        public Int64 orderbookTradeCount { get; set; }
        public Int64 removeCount { get; set; }
        public Int64 signatureVerificationCount { get; set; }
        public Int64 swapCount { get; set; }
        public Int64 swapNFTCount { get; set; }
        public string? timestamp { get; set; }
        public Int64 tradeNFTCount { get; set; }
        public Int64 transactionCount { get; set; }
        public Int64 transferCount { get; set; }
        public Int64 transferNFTCount { get; set; }
        public string? txHash { get; set; }
        public Int64 withdrawalCount { get; set; }
        public Int64 withdrawalNFTCount { get; set; }
    }

    public class BlockData
    {
        public BlockDetail? block { get; set; }
        public Proxy? proxy { get; set; }
    }

    public class Block
    {
        public BlockData? data { get; set; }
    }

    public class BlocksData
    {
        public List<BlockDetail>? blocks { get; set; }
        public Proxy? proxy { get; set; }
    }

    public class Blocks
    {
        public BlocksData? data { get; set; }
    }
    public class Proxy
    {
        public Int64 accountUpdateCount { get; set; }
        public Int64 addCount { get; set; }
        public Int64 ammUpdateCount { get; set; }
        public Int64 depositCount { get; set; }
        public Int64 nftDataCount { get; set; }
        public Int64 nftMintCount { get; set; }
        public Int64 orderbookTradeCount { get; set; }
        public Int64 removeCount { get; set; }
        public Int64 signatureVerificationCount { get; set; }
        public Int64 swapCount { get; set; }
        public Int64 swapNFTCount { get; set; }
        public Int64 tradeNFTCount { get; set; }
        public Int64 blockCount { get; set; }
        public Int64 userCount { get; set; }
        public Int64 transactionCount { get; set; }
        public Int64 transferCount { get; set; }
        public Int64 transferNFTCount { get; set; }
        public Int64 withdrawalCount { get; set; }
        public Int64 withdrawalNFTCount { get; set; }
    }

    [JsonConverter(typeof(JsonSubtypes), "__typename")]
    public class Account
    {
        public string? address { get; set; }
        public string? id { get; set; }
        [JsonProperty("__typename")]
        public string? typeName { get; set; }
        public List<AccountBalance>? balances { get; set; }
    }

    public class Pool : Account
    {
        public int feeBipsAMM { get; set; }
    }
        
    public class Pair
    {
        public string? id { get; set; }
        public Token? token0 { get; set; }
        public Token? token1 { get; set; }
    }

    public class Token
    {
        public string? address { get; set; }
        public int decimals { get; set; }
        public string? id { get; set; }
        public string? name { get; set; }
        public string? symbol { get; set; }
    }

    public class AccountBalance
    {
        public Double balance { get; set; }
        public string? id { get; set; }
        public Token? token { get; set; }
        public Account? account { get; set; }
    }

    public class User : Account
    {
        public string? publicKey { get; set; }
    }

    [JsonConverter(typeof(JsonSubtypes), "__typename")]
    public class Transaction
    {
        public string? id { get; set; }
        public string? internalID { get; set; }
        [JsonProperty(PropertyName = "__typename")]
        public string? typeName { get; set; }
        public string? data { get; set; }
        public BlockDetail? block { get; set; }
        public List<AccountBalance>? tokenBalances { get; set; }
        public List<Account>? accounts { get; set; }
        public string verifiedAt
        {
            get
            {
                if (block == null) return string.Empty;

                return TimestampToUTCConverter.Convert(block.timestamp!);
            }
        }
        public Swap? swap { get { return this as Swap; } }
        public OrderBookTrade? orderBookTrade { get { return this as OrderBookTrade; } }
        public Transfer? transer { get { return this as Transfer; } }
    }

    public class Swap : Transaction
    {
        public Account? account { get; set; }
        public Pool? pool { get; set; }
        public Token? tokenA { get; set; }
        public Token? tokenB { get; set; }
        public Double tokenAPrice { get; set; }
        public Double tokenBPrice { get; set; }
        public Pair? pair { get; set; }
        public Double fillSA { get; set; }
        public Double fillSB { get; set; }
        public bool fillAmountBorSA { get; set; }
        public bool fillAmountBorSB { get; set; }
        public Double fillBA { get; set; }
        public Double fillBB { get; set; }
        public Double feeA { get; set; }
        public Double protocolFeeA { get; set; }
        public Double feeB { get; set; }
        public Double protocolFeeB { get; set; }
    }
    public class Deposit : Transaction
    {
        public Account? toAccount { get; set; }
        public Token? token { get; set; }
        public Double amount { get; set; }
    }

    public class OrderBookTrade : Transaction
    {
        public Account? accountA { get; set; }
        public Account? accountB { get; set; }
        public Token? tokenA { get; set; }
        public Token? tokenB { get; set; }
        public Double tokenAPrice { get; set; }
        public Double tokenBPrice { get; set; }
        public Pair? pair { get; set; }
        public Double fillSA { get; set; }
        public Double fillSB { get; set; }
        public bool fillAmountBorSA { get; set; }
        public bool fillAmountBorSB { get; set; }
        public Double fillBA { get; set; }
        public Double fillBB { get; set; }
        public Double feeA { get; set; }
        public Double protocolFeeA { get; set; }
        public Double feeB { get; set; }
        public Double protocolFeeB { get; set; }
    }

    public class Withdrawal : Transaction
    {
        public Account? fromAccount { get; set; }
        public Token? token { get; set; }
        public Token? feeToken { get; set; }
        public bool valid { get; set; }
        public Double amount { get; set; }
        public Double fee { get; set; }
    }
    public class Transfer : Transaction
    {
        public Account? fromAccount { get; set; }
        public Account? toAccount { get; set; }
        public Token? token { get; set; }
        public Token? feeToken { get; set; }
        public Double amount { get; set; }
        public Double fee { get; set; }
    }
    public class Add : Transaction
    {
        public Account? account { get;set; }
        public Pool? pool { get; set; }
        public Token? token { get; set; }
        public Token? feeToken { get; set; }
        public Double fee { get; set; }
        public Double amount { get; set; }
    }
    public class Remove : Transaction
    {
        public Account? account { get; set; }
        public Pool? pool { get; set; }
        public Token? token { get; set; }
        public Token? feeToken { get; set; }
        public Double fee { get; set; }
        public Double amount { get; set; }
    }
    public class AccountUpdate : Transaction
    {
        public User? user { get; set; }
        public Token? feeToken { get; set; }
        public Double fee { get; set; }
        public string? publicKey { get; set; }
        public int? nonce { get; set; }
    }

    public class TransactionsData
    {
        public Proxy? proxy { get; set; }
        public List<Transaction>? transactions { get; set; }
    }

    public class Transactions
    {
        public TransactionsData? data { get; set; }
    }

}
