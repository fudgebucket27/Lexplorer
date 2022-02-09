namespace Lexplorer.Helpers
{
    public static class ParameterHelper
    {
        public static string ConvertToTransactionLink(string transactionType, string transactionId, string? previousPageNumber = "0", string? originatingPage = "")
        {
            switch (transactionType)
            {
                case "Swap":
                    return $"transactions/Swap/{transactionId}?previousPageNumber={previousPageNumber}&originatingPage={originatingPage}";
                case "Transfer":
                    return $"transactions/Transfer/{transactionId}?previousPageNumber={previousPageNumber}&originatingPage={originatingPage}";
                case "OrderbookTrade":
                    return $"transactions/OrderbookTrade/{transactionId}?previousPageNumber={previousPageNumber}&originatingPage={originatingPage}";
                default:
                    return "404";
            }
        }
    }
}
