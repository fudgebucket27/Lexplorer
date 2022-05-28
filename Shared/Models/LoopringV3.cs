using Newtonsoft.Json;
using Lexplorer.Helpers;
using JsonSubTypes;
using System;
using System.Collections.Generic;
using System.Linq;

//source: https://thegraph.com/hosted-service/subgraph/loopring/loopring

namespace Lexplorer.Models
{
    public static class EnumerableExtension
    {
        //extension to get index with foreach
        //https://thomaslevesque.com/2019/11/18/using-foreach-with-index-in-c/
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }
    }

    public class BlockDetail
    {
        [JsonProperty("__typename")]
        public string? typeName { get; set; }
        public long accountUpdateCount { get; set; }
        public long addCount { get; set; }
        public long ammUpdateCount { get; set; }
        public string? blockHash { get; set; }
        public int blockSize { get; set; }
        public string? data { get; set; }
        public long depositCount { get; set; }
        public double gasLimit { get; set; }
        public double gasPrice { get; set; }
        public double height { get; set; }
        public string? id { get; set; }
        public long nftDataCount { get; set; }
        public long nftMintCount { get; set; }
        public Account? operatorAccount { get; set; }
        public long orderbookTradeCount { get; set; }
        public long removeCount { get; set; }
        public long signatureVerificationCount { get; set; }
        public long swapCount { get; set; }
        public long swapNFTCount { get; set; }
        public string? timestamp { get; set; }
        public long tradeNFTCount { get; set; }
        public long transactionCount { get; set; }
        public long transferCount { get; set; }
        public long transferNFTCount { get; set; }
        public string? txHash { get; set; }
        public long withdrawalCount { get; set; }
        public long withdrawalNFTCount { get; set; }
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
        public long accountUpdateCount { get; set; }
        public long addCount { get; set; }
        public long ammUpdateCount { get; set; }
        public long depositCount { get; set; }
        public long nftDataCount { get; set; }
        public long nftMintCount { get; set; }
        public long orderbookTradeCount { get; set; }
        public long removeCount { get; set; }
        public long signatureVerificationCount { get; set; }
        public long swapCount { get; set; }
        public long swapNFTCount { get; set; }
        public long tradeNFTCount { get; set; }
        public long blockCount { get; set; }
        public long userCount { get; set; }
        public long transactionCount { get; set; }
        public long transferCount { get; set; }
        public long transferNFTCount { get; set; }
        public long withdrawalCount { get; set; }
        public long withdrawalNFTCount { get; set; }
    }

    [JsonConverter(typeof(JsonSubtypes), "__typename")]
    public class Account
    {
        public string? address { get; set; }
        public string? id { get; set; }
        [JsonProperty("__typename")]
        public string? typeName { get; set; }
        public List<AccountTokenBalance>? balances { get; set; }
        public Transaction? createdAtTransaction { get; set; }
    }
    public class Pool : Account
    {
        public int? feeBipsAMM { get; set; }
    }
    public class Pair
    {
        public string? id { get; set; }
        public Token? token0 { get; set; }
        public Token? token1 { get; set; }
        public double token0Price { get; set; }
        public double token1Price { get; set; }
        public double tradedVolumeToken0 { get; set; }
        public double tradedVolumeToken1 { get; set; }
        public double tradedVolumeToken0Swap { get; set; }
        public double tradedVolumeToken1Swap { get; set; }
        public double tradedVolumeToken0Orderbook { get; set; }
        public double tradedVolumeToken1Orderbook { get; set; }

        public List<PairDailyData>? dailyEntities { get; set; }
        public List<PairWeeklyData>? weeklyEntities { get; set; }
    }
    public class Token
    {
        public string? address { get; set; }
        public int decimals { get; set; }
        public string? id { get; set; }
        public string? name { get; set; }
        public string? symbol { get; set; }
    }

    public class AccountTokenBalance
    {
        public double balance { get; set; }
        public string? id { get; set; }
        public Token? token { get; set; }
        public Account? account { get; set; }
        public double fBalance {
            get
            {
                return balance / Math.Pow(10, token!.decimals);
            }
        }
    }

    public class User : Account
    {
        public string? publicKey { get; set; }
    }

    public class ProtocolAccount : Account
    {

    }

    [JsonConverter(typeof(JsonSubtypes), "__typename")]
    public class Transaction
    {
        public static readonly List<string> typeNames = new List<string> {
            "Swap",
            "MintNFT",
            "OrderbookTrade",
            "Deposit",
            "Withdrawal",
            "WithdrawalNFT",
            "Transfer",
            "TransferNFT",
            "Add",
            "Remove",
            "TradeNFT",
            "SwapNFT",
            "AccountUpdate",
            "AmmUpdate",
            "SignatureVerification",
            "DataNFT"};

        public string? id { get; set; }
        public string? internalID { get; set; }
        [JsonProperty(PropertyName = "__typename")]
        public string? typeName { get; set; }
        public string? data { get; set; }
        public BlockDetail? block { get; set; }
        public List<AccountTokenBalance>? tokenBalances { get; set; }
        public List<Account>? accounts { get; set; }
        public string? verifiedAt
        {
            get
            {
                if (block == null) return string.Empty;

                return TimestampConverter.ToUTCString(block.timestamp);
            }
        }
        public DateTime? verifiedAtDateTime
        {
            get
            {
                return TimestampConverter.ToUTCDateTime(block?.timestamp);
            }
        }
    }

    public class Swap : Transaction
    {
        public Account? account { get; set; }
        public Pool? pool { get; set; }
        public Token? tokenA { get; set; }
        public Token? tokenB { get; set; }
        public double tokenAPrice { get; set; }
        public double tokenBPrice { get; set; }
        public Pair? pair { get; set; }
        public double fillSA { get; set; }
        public double fillSB { get; set; }
        public bool fillAmountBorSA { get; set; }
        public bool fillAmountBorSB { get; set; }
        public double fillBA { get; set; }
        public double fillBB { get; set; }
        public double feeA { get; set; }
        public double protocolFeeA { get; set; }
        public double feeB { get; set; }
        public double protocolFeeB { get; set; }
    }
    public class Deposit : Transaction
    {
        public Account? toAccount { get; set; }
        public Token? token { get; set; }
        public double amount { get; set; }
    }

    public class OrderBookTrade : Transaction
    {
        public Account? accountA { get; set; }
        public Account? accountB { get; set; }
        public Token? tokenA { get; set; }
        public Token? tokenB { get; set; }
        public double tokenAPrice { get; set; }
        public double tokenBPrice { get; set; }
        public Pair? pair { get; set; }
        public double fillSA { get; set; }
        public double fillSB { get; set; }
        public bool fillAmountBorSA { get; set; }
        public bool fillAmountBorSB { get; set; }
        public double fillBA { get; set; }
        public double fillBB { get; set; }
        public double feeA { get; set; }
        public double protocolFeeA { get; set; }
        public double feeB { get; set; }
        public double protocolFeeB { get; set; }
    }

    public class Withdrawal : Transaction
    {
        public Account? fromAccount { get; set; }
        public Token? token { get; set; }
        public Token? feeToken { get; set; }
        public bool valid { get; set; }
        public double amount { get; set; }
        public double fee { get; set; }
    }
    public class Transfer : Transaction
    {
        public Account? fromAccount { get; set; }
        public Account? toAccount { get; set; }
        public Token? token { get; set; }
        public Token? feeToken { get; set; }
        public double amount { get; set; }
        public double fee { get; set; }
    }
    public class Add : Transaction
    {
        public Account? account { get; set; }
        public Pool? pool { get; set; }
        public Token? token { get; set; }
        public Token? feeToken { get; set; }
        public double fee { get; set; }
        public double amount { get; set; }
    }
    public class Remove : Transaction
    {
        public Account? account { get; set; }
        public Pool? pool { get; set; }
        public Token? token { get; set; }
        public Token? feeToken { get; set; }
        public double fee { get; set; }
        public double amount { get; set; }
    }
    public class AmmUpdate : Transaction
    {
        public Pool? pool { get; set; }
        public double tokenWeight { get; set; }
        public string? tokenID { get; set; }
        public double balance { get; set; }
    }
    public class AccountUpdate : Transaction
    {
        public User? user { get; set; }
        public Token? feeToken { get; set; }
        public double fee { get; set; }
        public string? publicKey { get; set; }
        public int? nonce { get; set; }
    }
    public class NonFungibleToken
    {
        public string? id { get; set; }
        [JsonProperty("__typename")]
        public string? typeName { get; set; }
        public MintNFT? mintedAtTransaction { get; set; }
        public Account? minter { get; set; }
        public string? token { get; set; }
        public string? nftID { get; set; }
        public int nftType { get; set; }
        public string nftTypeName
        {
            get
            {
                switch (nftType)
                {
                    case 0:
                        return "ERC1155";
                    case 1:
                        return "ERC721";
                    default:
                        return "unknown";
                }
            }
        }
        public List<AccountNFTSlot>? slots { get; set; }
        public List<TransactionNFT>? transactions { get; set; }
    }
    public class AccountNFTSlot
    {
        public string? id { get; set; }
        [JsonProperty("__typename")]
        public string? typeName { get; set; }
        public Account? account { get; set; }
        public double balance { get; set; }
        public Transaction? createdAtTransaction { get; set; }
        public Transaction? lastUpdatedAtTransaction { get; set; }
        public List<Transaction>? transactions { get; set; }
        public NonFungibleToken? nft { get; set; }
    }

    public class TransactionNFT : Transaction
    {
        public List<NonFungibleToken>? nfts { get; set; }
        public List<AccountNFTSlot>? slots { get; set; }
    }

    public class MintNFT : TransactionNFT
    {
        public AccountNFTSlot? receiverSlot { get; set; }
        public Account? minter { get; set; }
        public Account? receiver { get; set; }
        public NonFungibleToken? nft { get; set; }
        public Token? feeToken { get; set; }
        public double fee { get; set; }
        public double amount { get; set; }
        public string? extraData { get; set; }
    }

    public class WithdrawalNFT : TransactionNFT
    {
        public Account? fromAccount { get; set; }
        public AccountNFTSlot? slot { get; set; }
        public Token? feeToken { get; set; }
        public bool valid { get; set; }
        public double amount { get; set; }
        public double fee { get; set; }
    }

    public class TransferNFT : TransactionNFT
    {
        public Account? fromAccount { get; set; }
        public Account? toAccount { get; set; }
        public AccountNFTSlot? fromSlot { get; set; }
        public AccountNFTSlot? toSlot { get; set; }
        public Token? feeToken { get; set; }
        public double amount { get; set; }
        public double fee { get; set; }
    }

    public class TradeNFT : TransactionNFT
    {
        public Account? accountSeller { get; set; }
        public Account? accountBuyer { get; set; }
        public AccountNFTSlot? slotSeller { get; set; }
        public AccountNFTSlot? slotBuyer { get; set; }
        public Token? token { get; set; }
        public double realizedNFTPrice { get; set; }
        public double fillSA { get; set; }
        public double fillSB { get; set; }
        public bool fillAmountBorSA { get; set; }
        public bool fillAmountBorSB { get; set; }
        public double fillBA { get; set; }
        public double fillBB { get; set; }
        public double feeSeller { get; set; }
        public double feeBuyer { get; set; }
    }
    public class SwapNFT : TransactionNFT
    {
        public Account? accountA { get; set; }
        public Account? accountB { get; set; }
        public AccountNFTSlot? slotASeller { get; set; }
        public AccountNFTSlot? slotBSeller { get; set; }
        public AccountNFTSlot? slotABuyer { get; set; }
        public AccountNFTSlot? slotBBuyer { get; set; }
        public double fillSA { get; set; }
        public double fillSB { get; set; }
        public bool fillAmountBorSA { get; set; }
        public bool fillAmountBorSB { get; set; }
        public double fillBA { get; set; }
        public double fillBB { get; set; }
        public double feeSeller { get; set; }
        public double feeBuyer { get; set; }

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

    public class Pairs
    {
        public PairsData? data { get; set; }
    }

    public class PairsData
    {
        public List<Pair>? pairs { get; set; }
    }

    public class PairEntity
    {
        public string? id { get; set; }

        public double token0PriceLow { get; set; }
        public double token0PriceHigh { get; set; }
        public double token0PriceOpen { get; set; }
        public double token0PriceClose { get; set; }

        public double token1PriceLow { get; set; }
        public double token1PriceHigh { get; set; }
        public double token1PriceOpen { get; set; }
        public double token1PriceClose { get; set; }

        public double tradedVolumeToken0 { get; set; }
        public double tradedVolumeToken0Swap { get; set; }
        public double tradedVolumeToken0Orderbook { get; set; }
        public double tradedVolumeToken1 { get; set; }
        public double tradedVolumeToken1Swap { get; set; }
        public double tradedVolumeToken1Orderbook { get; set; }
    }

    public class PairDailyData : PairEntity
    {
        public double dayStart { get; set; }
        public double dayEnd { get; set; }
        public double dayNumber { get; set; }

        //Day number is the amount of days since the start block of Loopring 3.6 (block 11149814)
        //https://etherscan.io/block/11149814
        //Oct-29-2020 05:41:42 AM +UTC
        private static readonly DateTime _LRUTCStartBlock = new(2020, 10, 29, 05, 41, 42, DateTimeKind.Utc);

        public DateTime dayDateTime => _LRUTCStartBlock.AddDays(dayNumber);
    }

    public class PairWeeklyData : PairEntity
    {
        public double weekStart { get; set; }
        public double weekEnd { get; set; }
        public double weekNumber { get; set; }
    }

}
