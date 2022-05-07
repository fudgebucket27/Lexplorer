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
    public class TestPairs
    {
        GraphQLTestsFixture fixture;
        LoopringGraphQLService service;

        public TestPairs(GraphQLTestsFixture fixture)
        {
            this.fixture = fixture;
            service = fixture!.LGS;
        }

        [Fact]
        public async void GetPairs()
        {
            var pairs = await service.GetPairs(0, 10);
            Assert.NotNull(pairs?.data);
            Assert.NotEmpty(pairs!.data!.pairs);
            Assert.Equal(10, pairs!.data!.pairs!.Count);
        }

        [Theory]
        [InlineData("0-1")]
        public async void GetPair(string pairID)
        {
            var pair = await service.GetPair(pairID);
            Assert.NotNull(pair);
            Assert.Equal(pairID, pair!.id);
        }

        [Theory]
        [InlineData("0-1", 0, 10)]
        public async void GetDailyEntities(string pairID, int skip, int first)
        {
            var entities = await service.GetPairDailyEntities(pairID, skip, first);
            Assert.NotEmpty(entities);
            Assert.Equal(first, entities!.Count);
        }
    }
}
