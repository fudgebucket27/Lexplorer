using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Lexplorer.Models;
using Lexplorer.Services;
using Lexplorer.Helpers;

namespace xUnitTests.LoopringGraphTests
{
    [Collection("LoopringGraphQL collection")]
    public class TestBlock
    {
        GraphQLTestsFixture fixture;
        LoopringGraphQLService service;

        public TestBlock(GraphQLTestsFixture fixture)
        {
            this.fixture = fixture;
            this.service = fixture!.LGS;
        }

        [Fact]
        public async void GetBlocks()
        {
            Blocks? blocks = await service.GetBlocks(0, 10);
            Assert.NotNull(blocks);
            Assert.NotEmpty(blocks!.data!.blocks!);
            Assert.Equal(10, blocks!.data!.blocks!.Count);
        }
        [Fact]
        public async void GetBlockByDate()
        {
            Blocks? blocks = await service.GetBlocks(0, 1, blockTimestamp: "1643909000", gte: false);
            Assert.NotNull(blocks);
            Assert.Single(blocks!.data!.blocks!);
            Assert.Equal("16791", blocks!.data!.blocks![0].id);
        }
        [Fact]
        public async void GetBlockDateRange()
        {
            Tuple<Double, Double>? blockIDs = await service.GetBlockDateRange(
                new DateTime(2022, 2, 20, 0, 0, 0, DateTimeKind.Utc), 
                new DateTime(2022, 2, 22, 0, 0, 0, DateTimeKind.Utc));
            Assert.NotNull(blockIDs);
            Assert.Equal(17282, blockIDs!.Item1);
            Assert.Equal(17341, blockIDs!.Item2);
        }
    }
}
