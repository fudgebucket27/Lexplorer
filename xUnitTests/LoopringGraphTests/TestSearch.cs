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

        [Theory]
        [InlineData("8d785aabf440e369aae5e63bed8a0f1f560b4caf")]
        public async void SearchL1Address(string address)
        {
            var searchResult = await service.Search(address);
            Assert.NotEmpty(searchResult);
            Assert.IsType<User>(searchResult![0]);
            Assert.Contains(address, (searchResult![0] as User)!.address);
        }
    }
}
