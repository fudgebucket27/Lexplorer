using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Lexplorer.Services;
using Lexplorer.Models;

namespace xUnitTests.LoopringGraphTests
{
    [Collection("LoopringGraphQL collection")]
    public class TestSearch
    {
        GraphQLTestsFixture fixture;
        LoopringGraphQLService service;

        public TestSearch(GraphQLTestsFixture fixture)
        {
            this.fixture = fixture;
            this.service = fixture!.LGS;
        }

        [Fact]
        public async void SearchAccountAndBlock()
        {
            var searchResult = await service.Search("17714");
            Assert.NotNull(searchResult);
            Assert.Equal(2, searchResult!.Count);
            Assert.IsType<User>(searchResult[0]);
            Assert.IsType<BlockDetail>(searchResult[1]);
        }

        [Fact]
        public async void SearchTransaction()
        {
            var searchResult = await service.Search("17714-19");
            Assert.NotNull(searchResult);
            Assert.Equal(1, searchResult!.Count);
            Assert.IsType<Add>(searchResult[0]);
        }
    }
}
