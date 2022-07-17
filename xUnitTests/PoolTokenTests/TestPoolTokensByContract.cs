using System;
using Xunit;
using Lexplorer.Models;

namespace xUnitTests.PoolTokenTests
{
    [Collection("PoolTokens collection")]
    public class TestPoolTokensByContract
	{
        readonly PoolTokensTestsFixture fixture;

        public TestPoolTokensByContract(PoolTokensTestsFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("84", "0xa573c5d473702286f0ac84592eda49ad799ebaa1", "LP-USDT-ETH")]
        [InlineData("85", "0x6d537764355bc23d4eadba7829048dac8215a73c", "LP-ETH-USDT")]
        [InlineData("86", "0x605872a5a459e778959b8a49dc3a56a8c9197983", "LP-ETH-USDT")]
        public async void TestPoolTokenByToken(string tokenID, string tokenAddress, string expectedSymbol)
        {
            var token = new Token();
            token.id = tokenID;
            token.address = tokenAddress;
            var poolToken = await fixture.LPTCS.GetPoolToken(token);

            Assert.NotNull(poolToken);
            Assert.NotNull(poolToken!.token);
            Assert.Equal(expectedSymbol, poolToken!.token!.symbol);
        }

        [Theory]
        [InlineData("2", "84", "LP-USDT-ETH")]
        [InlineData("3", "85", "LP-ETH-USDT")]
        [InlineData("4", "86", "LP-ETH-USDT")]
        public async void TestPoolTokenByPool(string poolID, string expectedTokenID, string expectedSymbol)
        {
            var pool = new Pool();
            pool.id = poolID;
            var poolToken = await fixture.LPTCS.GetPoolToken(pool);

            Assert.NotNull(poolToken);
            Assert.NotNull(poolToken!.token);
            Assert.Equal(expectedTokenID, poolToken!.token!.id);
            Assert.Equal(expectedSymbol, poolToken!.token!.symbol);
        }

        //cannot test same token 84 with a TestPoolTokenByPair as no pair for USDT-ETH exists
        //there is a pair the other way around but pair 0-3 actually has two pools!
        //pool 4, token 86 and abandoned pool 3, token 85!
        //since we're searching via swaps, we don't get the old pool token

        [Theory]
        [InlineData("85", "0x6d537764355bc23d4eadba7829048dac8215a73c", "3", "LP-ETH-USDT")]
        public async void TestDuplicatePoolToken(string tokenID, string tokenAddress, string poolID, string expectedSymbol)
        {
            //enable caching to actually try to add same token first directly, then via pool
            fixture.LPTCS.EnableCache();

            var token = new Token();
            token.id = tokenID;
            token.address = tokenAddress;
            var poolTokenDirect = await fixture.LPTCS.GetPoolToken(token);
            Assert.NotNull(poolTokenDirect);
            Assert.NotNull(poolTokenDirect!.token);
            Assert.Equal(tokenID, poolTokenDirect!.token!.id);
            Assert.Equal(expectedSymbol, poolTokenDirect!.token!.symbol);

            var pool = new Pool();
            pool.id = poolID;
            var poolTokenViaPool = await fixture.LPTCS.GetPoolToken(pool);

            Assert.NotNull(poolTokenViaPool);
            Assert.NotNull(poolTokenViaPool!.token);
            Assert.Equal(tokenID, poolTokenViaPool!.token!.id);
            Assert.Equal(expectedSymbol, poolTokenViaPool!.token!.symbol);

            Assert.NotNull(poolTokenViaPool.pool);
            Assert.Equal(poolID, poolTokenViaPool.pool!.id);

            fixture.LPTCS.DisableCache();
        }

    }
}

