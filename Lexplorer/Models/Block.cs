namespace Lexplorer.Models
{
    public class BlockOperatorAccount
    {
        public string address { get; set; }
        public string id { get; set; }
    }

    public class BlockDetail
    {
        public string accountUpdateCount { get; set; }
        public string addCount { get; set; }
        public string ammUpdateCount { get; set; }
        public string blockHash { get; set; }
        public int blockSize { get; set; }
        public string data { get; set; }
        public string depositCount { get; set; }
        public string gasLimit { get; set; }
        public string gasPrice { get; set; }
        public string height { get; set; }
        public string id { get; set; }
        public string nftDataCount { get; set; }
        public string nftMintCount { get; set; }
        public BlockOperatorAccount operatorAccount { get; set; }
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
        public string txHash { get; set; }
        public string withdrawalCount { get; set; }
        public string withdrawalNFTCount { get; set; }
    }

    public class BlockProxy
    {
        public string blockCount { get; set; }
    }

    public class BlockData
    {
        public BlockDetail block { get; set; }
        public BlockProxy proxy { get; set; }
    }

    public class Block
    {
        public BlockData data { get; set; }

        public Block(BlockData data)
        {
            this.data = data;
        }
    }


}
