namespace Lexplorer.Helpers
{
    public static class ParameterHelper
    {
        public static string ConvertToTransactionLink(string transactionType, string transactionId, string? previousPageNumber = "0")
        {
            switch (transactionType)
            {
                case "Swap":
                    return $"transactions/Swap/{transactionId}?previousPageNumber={previousPageNumber}";
                case "Transfer":
                    return $"transactions/Transfer/{transactionId}?previousPageNumber={previousPageNumber}";
                case "OrderbookTrade":
                    return $"transactions/OrderbookTrade/{transactionId}?previousPageNumber={previousPageNumber}";
                default:
                    return "404";
            }
        }
    }
}
