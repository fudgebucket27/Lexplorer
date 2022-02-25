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
    public class TestTokenAmountConverterConvert
    {
        [Fact]
        public void TestConvertSimple()
        {
            Assert.Equal("100", TokenAmountConverter.Convert(1E5, 3));
        }
    }

    public class TestTokenAmountConverterToKMB
    {
        Double x = 12345678;

        [Fact]
        public void TestToKMBMillons()
        {
            Assert.Equal("1.23M", TokenAmountConverter.ToKMB(x, 1, 1));
        }
        [Fact]
        public void TestToKMBBillons()
        {
            Assert.Equal("1.235B", TokenAmountConverter.ToKMB(x, -2, 1));
        }
        [Fact]
        public void TestToKMBKilo()
        {
            Assert.Equal("1.2K", TokenAmountConverter.ToKMB(x, 4, 1));
        }
        [Fact]
        public void TestToKMBNoPrefix()
        {
            Assert.Equal("1.23", TokenAmountConverter.ToKMB(x, 7, 1));
        }
    }
}
