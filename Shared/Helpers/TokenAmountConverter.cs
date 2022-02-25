using System;
using System.Globalization;

namespace Lexplorer.Helpers
{
    public static class TokenAmountConverter
    {
        public static decimal ToDecimal(Double balance, int decimals, decimal conversionRate = 1)
        {
            return (decimal) (balance / Math.Pow(10, (double)decimals)) * conversionRate; 
        }
        public static string Convert(Double balance, int decimals, decimal conversionRate = 1)
        {
            return ToDecimal(balance, decimals, conversionRate).ToString();
        }

        public static decimal DecimalWithExponent(decimal amount, out string exponentPrefix)
        {
            exponentPrefix = "";
            if (amount == 0) return amount;

            //get the exponent - sign doesn't matter, i.e. 6 for 1,000,000 aka 1E6
            var exponent = Math.Log10((double)Math.Abs(amount));

            //we since we're only interested in k, M and B, keep it simple
            if (exponent >= 9)
            {
                exponentPrefix = "B";
                return amount / (decimal)1E9;
            }
            else if (exponent >= 6)
            {
                exponentPrefix = "M";
                return amount / (decimal)1E6;
            }
            else if (exponent >= 3)
            {
                exponentPrefix = "k";
                return amount / (decimal)1E3;
            }
            else
                return amount;
        }

        public static string ToKMB(double num, int decimals, decimal conversionRate, string format = "N3")
        {
            string expPrefix = "";
            decimal amount = DecimalWithExponent(ToDecimal(num, decimals, conversionRate), out expPrefix);
            return amount.ToString(format, CultureInfo.InvariantCulture) + expPrefix;
        }
    }
}
