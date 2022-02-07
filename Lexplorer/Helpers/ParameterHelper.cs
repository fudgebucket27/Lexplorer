namespace Lexplorer.Helpers
{
    public static class ParameterHelper
    {
        public static string ConvertToTransactionLink(string transactionType, string transactionId)
        {
            switch (transactionType)
            {
                case "Swap":
                    return $"transaction/Swap/{transactionId}";
                default:
                    return "";
            }
        }
    }
}
