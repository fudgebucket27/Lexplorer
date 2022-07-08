using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTests.EthereumServiceTests
{
    [Collection("EthereumTests collection")]
    public class TestEthereumService
    {
        readonly EthereumTestsFixture fixture;

        public TestEthereumService(EthereumTestsFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("fudgey.loopring.eth")]
        [InlineData("fudgey.eth")]
        [InlineData("bubblecum.eth")]
        public async void TestGetHexAddressFromsEns(string ens)
        {
            var hexAddress = await fixture.EthS.GetEthAddressFromEns(ens);
            Assert.NotNull(hexAddress);
        }
    }
}
