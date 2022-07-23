using System;
using Xunit;
using Lexplorer.Models;

namespace xUnitTests.PoolTokenTests
{
    [Collection("PoolTokens collection")]
    public class TestPoolTokensByGraphQL
	{
        readonly PoolTokensTestsFixture fixture;

        public TestPoolTokensByGraphQL(PoolTokensTestsFixture fixture)
		{
            this.fixture = fixture;
		}

        private static Token TokenCreationHelper(string id, string symbol)
        {
            var token = new Token();
            token.id = id;
            token.symbol = symbol;
            return token;
        }

        [Theory]
        [InlineData("83", "LP-ETH-LRC")]
        public async void TestPoolTokenByToken(string tokenID, string expectedSymbol)
        {
            var token = new Token();
            token.id = tokenID;
            var poolToken = await fixture.LPTCS.GetPoolToken(token);

            Assert.NotNull(poolToken);
            Assert.NotNull(poolToken!.token);
            Assert.Equal(expectedSymbol, poolToken!.token!.symbol);
        }

        [Theory]
        [InlineData("1", "LP-ETH-LRC")]
        public async void TestPoolTokenByPool(string poolID, string expectedSymbol)
        {
            var pool = new Pool();
            pool.id = poolID;
            var poolToken = await fixture.LPTCS.GetPoolToken(pool);

            Assert.NotNull(poolToken);
            Assert.NotNull(poolToken!.token);
            Assert.Equal(expectedSymbol, poolToken!.token!.symbol);
        }

        [Theory]
        [InlineData("0-1", "0", "ETH", "1", "LRC", "LP-ETH-LRC")]
        [InlineData("0-3", "0", "ETH", "3", "USDT", "LP-ETH-USDT")]
        public async void TestPoolTokenByPair(string pairID, string tokenID0, string tokenSymbol0, string tokenID1, string tokenSymbol1, string expectedSymbol)
        {
            var pair = new Pair();
            pair.id = pairID;
            pair.token0 = TokenCreationHelper(tokenID0, tokenSymbol0);
            pair.token1 = TokenCreationHelper(tokenID1, tokenSymbol1);
            var poolToken = await fixture.LPTCS.GetPoolToken(pair);

            Assert.NotNull(poolToken);
            Assert.NotNull(poolToken!.token);
            Assert.Equal(expectedSymbol, poolToken!.token!.symbol);
        }

        [Theory]
        [InlineData("25279-252", "LP-ETH-LRC")]
        public async void TestPoolTokenBySwap(string swapID, string expectedSymbol)
        {
            var swap = new Swap();
            swap.id = swapID;
            var poolToken = await fixture.LPTCS.GetPoolToken(swap);

            Assert.NotNull(poolToken);
            Assert.NotNull(poolToken!.token);
            Assert.Equal(expectedSymbol, poolToken!.token!.symbol);
        }
    }
}

