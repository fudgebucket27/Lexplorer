namespace Lexplorer.Models
{
    public class TransferBlock
    {
        public string id { get; set; }
        public string blockHash { get; set; }
        public string timestamp { get; set; }
        public string transactionCount { get; set; }
    }

    public class TransferToken
    {
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public int decimals { get; set; }
        public string address { get; set; }
    }

    public class TransferFeeToken
    {
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public int decimals { get; set; }
        public string address { get; set; }
    }

    public class TransferTransaction
    {
        public string id { get; set; }
        public TransferBlock block { get; set; }
        public string data { get; set; }
        public Account fromAccount { get; set; }
        public Account toAccount { get; set; }
        public TransferToken token { get; set; }
        public TransferFeeToken feeToken { get; set; }
        public string amount { get; set; }
        public string fee { get; set; }
        public string __typename { get; set; }
    }

    public class TransferData
    {
        public TransferTransaction transaction { get; set; }
    }

    public class Transfer
    {
        public TransferData data { get; set; }
    }


}
