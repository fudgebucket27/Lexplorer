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
                default:
                    return "404";
            }
        }
    }
}
