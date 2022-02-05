namespace Lexplorer.Helpers
{
    public static class TokenAmountConverter
    {
        public static string Convert(string balance, int decimals)
        {
            var result = Double.Parse(balance) / Math.Pow(10, (double)decimals);
            return result.ToString();
        }
    }
}
