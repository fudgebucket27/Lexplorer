namespace Lexplorer.Helpers
{
    public static class TokenAmountConverter
    {
        public static string Convert(Double balance, int decimals)
        {
            var result = balance / Math.Pow(10, (double)decimals);
            return result.ToString();
        }
    }
}
