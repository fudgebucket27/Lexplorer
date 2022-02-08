namespace Lexplorer.Helpers
{
    public static class ParameterHelper
    {
        public static string ConvertToTransactionLink(string transactionType, string transactionId, string? previousPageNumber = "0")
        {
            switch (transactionType)
            {
                case "Swap":
                    return $"transaction/Swap/{transactionId}?previousPageNumber={previousPageNumber}";
                case "Transfer":
                    return $"transaction/Transfer/{transactionId}?previousPageNumber={previousPageNumber}";
                default:
                    return "404";
            }
        }
    }
}
