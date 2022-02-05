namespace Lexplorer.Models
{
    public class OperatorAccount
    {
        public string address { get; set; }
        public string id { get; set; }
    }

    public class Block
    {
        public string blockHash { get; set; }
        public int blockSize { get; set; }
        public string gasLimit { get; set; }
        public string gasPrice { get; set; }
        public string height { get; set; }
        public string id { get; set; }
        public OperatorAccount operatorAccount { get; set; }
        public string timestamp { get; set; }
        public string transactionCount { get; set; }
        public string txHash { get; set; }
    }

    public class Proxy
    {
        public string blockCount { get; set; }
        public string userCount { get; set; }
    }

    public class Data
    {
        public List<Block> blocks { get; set; }
        public Proxy proxy { get; set; }
    }

    public class BlockData
    {
        public Data data { get; set; }
    }
}
