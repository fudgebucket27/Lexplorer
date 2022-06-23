using System;
using Lexplorer.Models;
using Lexplorer.Services;
using Xunit;

namespace xUnitTests.LoopringGraphTests
{
    [Collection("LoopringGraphQL collection")]
    public class TestSwapPairAndPool
	{
        GraphQLTestsFixture fixture;
        LoopringGraphQLService service;

        public TestSwapPairAndPool(GraphQLTestsFixture fixture)
        {
            this.fixture = fixture;
            service = fixture!.LGS;
        }

        [Theory]
        [InlineData("99")]
        public async void GetSwapWithPool(string poolID)
        {
            Pool pool = new();
            pool.id = poolID;
            var swap = await service.GetSwapPairAndPool(pool);
            Assert.NotNull(swap);
            Assert.NotNull(swap!.pool);
            Assert.NotNull(swap.pool!.balances);
            Assert.NotNull(swap.pair);
            Assert.NotNull(swap.pair!.token0);
            Assert.NotNull(swap.pair.token1);
            Assert.Equal(poolID, swap!.pool!.id);
        }

        [Theory]
        [InlineData("0-217")]
        public async void GetSwapWithPair(string pairID)
        {
            Pair pair = new();
            pair.id = pairID;
            var swap = await service.GetSwapPairAndPool(pair);
            Assert.NotNull(swap);
            Assert.NotNull(swap!.pool);
            Assert.NotNull(swap.pool!.balances);
            Assert.NotNull(swap.pair);
            Assert.NotNull(swap.pair!.token0);
            Assert.NotNull(swap.pair.token1);
            Assert.Equal(pairID, swap!.pair!.id);
        }

        [Theory]
        [InlineData("10004-35")]
        public async void GetSwapWithSwap(string swapID)
        {
            Swap querySwap = new();
            querySwap.id = swapID;
            var swap = await service.GetSwapPairAndPool(querySwap);
            Assert.NotNull(swap);
            Assert.NotNull(swap!.pool);
            Assert.NotNull(swap.pool!.balances);
            Assert.NotNull(swap.pair);
            Assert.NotNull(swap.pair!.token0);
            Assert.NotNull(swap.pair.token1);
            Assert.Equal(swapID, swap.id);
        }

    }
}
