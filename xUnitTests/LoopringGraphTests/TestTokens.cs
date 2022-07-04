using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Lexplorer.Services;

namespace xUnitTests.LoopringGraphTests
{
    [Collection("LoopringGraphQL collection")]
    public class TestTokens
    {
        GraphQLTestsFixture fixture;
        LoopringGraphQLService service;

        public TestTokens(GraphQLTestsFixture fixture)
        {
            this.fixture = fixture;
            service = fixture!.LGS;
        }

        [Fact]
        public async void GetTokens()
        {
            var tokens = await service.GetTokens(0, 10);
            Assert.NotEmpty(tokens);
            Assert.Equal(10, tokens!.Count);
        }

        [Theory]
        [InlineData("1", "LRC", "0xbbbbca6a901c926f240b89eacb641d8aec7aeafd")]
        [InlineData("0", "ETH", "0x0000000000000000000000000000000000000000")]
        public async void GetPair(string tokenID, string tokenSymbol, string tokenAddress)
        {
            var token = await service.GetToken(tokenID);
            Assert.NotNull(token);
            Assert.Equal(tokenSymbol, token!.symbol);
            Assert.Equal(tokenAddress, token!.address);
        }

    }
}
