using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Lexplorer.Helpers;
using System.Globalization;

namespace xUnitTests.HelperTests
{
    public class TestTokenAmountConverterToDecimal
    {
        Double x = 12345678;

        [Fact]
        public void TestToDecimalSimple()
        {
            Assert.Equal((decimal) x / 1000, TokenAmountConverter.ToDecimal(x, 3), 0);
        }
        [Fact]
        public void TestConvertSimpleConversion()
        {
            Assert.Equal((decimal)x / 4000, TokenAmountConverter.ToDecimal(x, 3, (decimal)1 / 4), 0);
        }
    }
    public class TestTokenAmountConverterConvert
    {
        [Fact]
        public void TestConvertSimple()
        {
            Assert.Equal("100", TokenAmountConverter.Convert(1E5, 3));
        }
        [Fact]
        public void TestConvertSimpleConversion()
        {
            Assert.Equal("100,00", TokenAmountConverter.Convert(1E5 * 4, 3, (decimal)1/4));
        }
    }

    public class TestTokenAmountConverterDecimalWithExponent
    {
        [Fact]
        public void TestZero()
        {
            string ExpPrefix = "";

            Assert.Equal(0, TokenAmountConverter.DecimalWithExponent(0, out ExpPrefix));
            Assert.Equal("", ExpPrefix);
        }
        [Fact]
        public void TestSubOneK()
        {
            string ExpPrefix = "";

            Assert.Equal(985, TokenAmountConverter.DecimalWithExponent(985, out ExpPrefix));
            Assert.Equal("", ExpPrefix);
        }
        [Fact]
        public void TestAtOneK()
        {
            string ExpPrefix = "";

            Assert.Equal(1, TokenAmountConverter.DecimalWithExponent(1000, out ExpPrefix));
            Assert.Equal("k", ExpPrefix);
        }
        [Fact]
        public void TestSomeK()
        {
            string ExpPrefix = "";

            Assert.Equal(3.498m, TokenAmountConverter.DecimalWithExponent(3498, out ExpPrefix));
            Assert.Equal("k", ExpPrefix);
        }
        [Fact]
        public void TestSomeM()
        {
            string ExpPrefix = "";

            Assert.Equal(7.568m, TokenAmountConverter.DecimalWithExponent(7568000, out ExpPrefix));
            Assert.Equal("M", ExpPrefix);
        }
        [Fact]
        public void TestSomeB()
        {
            string ExpPrefix = "";

            Assert.Equal(1.258765m, TokenAmountConverter.DecimalWithExponent(1258765000, out ExpPrefix));
            Assert.Equal("B", ExpPrefix);
        }
    }

    public class TestTokenAmountConverterToKMB
    {
        Double x = 12345678;

        [Fact]
        public void TestToKMBMillons()
        {
            Assert.Equal("1.235M", TokenAmountConverter.ToKMB(x, 1, 1));
        }
        [Fact]
        public void TestToKMBBillons()
        {
            Assert.Equal("1.235B", TokenAmountConverter.ToKMB(x, -2, 1));
        }
        [Fact]
        public void TestToKMBKilo()
        {
            Assert.Equal("1.235k", TokenAmountConverter.ToKMB(x, 4, 1));
        }
        [Fact]
        public void TestToKMBNoPrefix()
        {
            Assert.Equal("1.235", TokenAmountConverter.ToKMB(x, 7, 1));
        }
    }
}
