using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Lexplorer.Helpers;

namespace xUnitTests.HelperTests
{
    public class TestTimestampConverter
    {
        [Fact]
        public void TestRoundtrip()
        {
            DateTime input = new DateTime(2022, 02, 20, 0, 0, 0, DateTimeKind.Utc);
            DateTime? output = TimestampConverter.ToUTCDateTime(TimestampConverter.ToTimeStamp(input).ToString());
            Assert.NotNull(output);
            Assert.Equal(input, output);
        }
    }
}
