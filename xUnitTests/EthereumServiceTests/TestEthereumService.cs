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

        [Theory]
        [InlineData("0x69a8bdEE1af2138C58B1261373B37071850689C0", "AMM-MKR-ETH", "LP-MKR-ETH")]
        [InlineData("0x9f8F72aA9304c8B593d555F12eF6589cC3A579A2", "Maker", "MKR")]
        public async void TestGetTokenInfo(string contract, string expectedName, string expectedSymbol)
        {
            var name = await fixture.EthS.GetTokenNameFromAddress(contract);
            //Assert.Equal(expectedName, name);
            var symbol = await fixture.EthS.GetTokenSymbolFromAddress(contract);
            Assert.Equal(expectedSymbol, symbol);
        }
    }
}
