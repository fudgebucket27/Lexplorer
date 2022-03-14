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
            this.service = fixture!.LGS;
        }

        [Fact]
        public async void GetPairs()
        {
            var pairs = await service.GetPairs(0, 10);
            Assert.NotNull(pairs?.data);
            Assert.NotEmpty(pairs!.data!.pairs);
            Assert.Equal(10, pairs!.data!.pairs!.Count);
        }
    }
}
