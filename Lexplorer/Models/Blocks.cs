namespace Lexplorer.Models
{
    public class BlocksOperatorAccount
    {
        public string address { get; set; }
        public string id { get; set; }
    }

    public class BlocksDetail
    {
        public string blockHash { get; set; }
        public int blockSize { get; set; }
        public string gasLimit { get; set; }
        public string gasPrice { get; set; }
        public string height { get; set; }
        public string id { get; set; }
        public BlocksOperatorAccount operatorAccount { get; set; }
        public string timestamp { get; set; }
        public string transactionCount { get; set; }
        public string txHash { get; set; }
    }

    public class BlocksProxy
    {
        public string blockCount { get; set; }
        public string userCount { get; set; }
    }

    public class BlockData
    {
        public List<BlocksDetail> blocks { get; set; }
        public BlocksProxy proxy { get; set; }
    }

    public class Blocks
    {
        public BlockData data { get; set; }
    }
}
