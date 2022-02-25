using System;
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


        public static string ToKMB(double num, int decimals, decimal conversionRate)
        {
            num = num / Math.Pow(10, (double)decimals);
            var result = (decimal) num * conversionRate;
            if (result > 999999999 || num < -999999999)
            {
                return result.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else
            if (result > 999999 || num < -999999)
            {
                return result.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else
            if (result > 999 || result < -999)
            {
                return result.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return result.ToString("0.##",CultureInfo.InvariantCulture);
            }
        }
    }
}
