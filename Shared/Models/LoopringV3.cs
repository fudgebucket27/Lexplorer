using Newtonsoft.Json;
using Lexplorer.Helpers;
using JsonSubTypes;
using System;
using System.Collections.Generic;

//source: https://thegraph.com/hosted-service/subgraph/loopring/loopring

namespace Lexplorer.Models
{
    public class BlockDetail
    {
        [JsonProperty("__typename")]
        public string? typeName { get; set; }
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
        public List<AccountTokenBalance>? balances { get; set; }
        public Transaction? createdAtTransaction { get; set; }
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

        public List<DailyEntity>? dailyEntities { get; set; }
        public List<WeeklyEntity>? weeklyEntities { get; set; }
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
        public Double balance { get; set; }
        public string? id { get; set; }
        public Token? token { get; set; }
        public Account? account { get; set; }
        public Double fBalance {
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
        public Account? account { get; set; }
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
    public class NonFungibleToken
    {
        public string? id { get; set; }
        [JsonProperty("__typename")]
        public string? typeName { get; set; }
        public MintNFT? mintedAtTransaction { get; set; }
        public Account? minter { get; set; }
        public string? token { get; set; }
        public string? nftID { get; set; }
        public List<AccountNFTSlot>? slots { get; set; }
        public List<TransactionNFT>? transactions { get; set; }
    }
    public class AccountNFTSlot
    {
        public string? id { get; set; }
        [JsonProperty("__typename")]
        public string? typeName { get; set; }
        public Account? account { get; set; }
        public Double balance { get; set; }
        public Transaction? createdAtTransaction { get; set; }
        public Transaction? lastUpdatedAtTransaction { get; set; }
        public List<Transaction>? transactions { get; set; }
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
        public Double fee { get; set; }
        public Double amount { get; set; }
        public string? extraData { get; set; }
    }
    public class WithDrawalNFT : TransactionNFT
    {
        public Account? fromAccount { get; set; }
        public AccountNFTSlot? slot { get; set; }
        public Token? feeToken { get; set; }
        public Boolean valid { get; set; }
        public Double amount { get; set; }
        public Double fee { get; set; }
    }
    public class TransferNFT : TransactionNFT
    {
        public Account? fromAccount { get; set; }
        public Account? toAccount { get; set; }
        public AccountNFTSlot? fromSlot { get; set; }
        public AccountNFTSlot? toSlot { get; set; }
        public Token? feeToken { get; set; }
        public Double amount { get; set; }
        public Double fee { get; set; }
    }
    public class TradeNFT : TransactionNFT
    {
        public Account? accountSeller { get; set; }
        public Account? accountBuyer { get; set; }
        public AccountNFTSlot? slotSeller { get; set; }
        public AccountNFTSlot? slotBuyer { get; set; }
        public Token? token { get; set; }
        public Double realizedNFTPrice { get; set; }
        public Double fillSA { get; set; }
        public Double fillSB { get; set; }
        public Boolean fillAmountBorSA { get; set; }
        public Boolean fillAmountBorSB { get; set; }
        public Double fillBA { get; set; }
        public Double fillBB { get; set; }
        public Double feeSeller { get; set; }
        public Double feeBuyer { get; set; }
    }
    public class SwapNFT : TransactionNFT
    {
        public Account? accountA { get; set; }
        public Account? accountB { get; set; }
        public AccountNFTSlot? slotASeller { get; set; }
        public AccountNFTSlot? slotBSeller { get; set; }
        public AccountNFTSlot? slotABuyer { get; set; }
        public AccountNFTSlot? slotBBuyer { get; set; }
        public Double fillSA { get; set; }
        public Double fillSB { get; set; }
        public Boolean fillAmountBorSA { get; set; }
        public Boolean fillAmountBorSB { get; set; }
        public Double fillBA { get; set; }
        public Double fillBB { get; set; }
        public Double feeSeller { get; set; }
        public Double feeBuyer { get; set; }

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

    public class DailyEntity
    {
        public Double tradedVolumeToken1Swap { get; set; }
        public Double tradedVolumeToken0Swap { get; set; }
        public string? id { get; set; }
    }

    public class WeeklyEntity
    {
        public Double tradedVolumeToken1Swap { get; set; }
        public Double tradedVolumeToken0Swap { get; set; }
        public string? id { get; set; }
    }




}
