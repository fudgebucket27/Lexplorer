using System.Globalization;

namespace Lexplorer.Helpers
{
    public static class TokenAmountConverter
    {
        public static string Convert(Double balance, int decimals)
        {
            var result = balance / Math.Pow(10, (double)decimals);
            return result.ToString();
        }


        public static string ToKMB(double num, int decimals)
        {
            num = num / Math.Pow(10, (double)decimals);
            if (num > 999999999 || num < -999999999)
            {
                return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999999 || num < -999999)
            {
                return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999 || num < -999)
            {
                return num.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
