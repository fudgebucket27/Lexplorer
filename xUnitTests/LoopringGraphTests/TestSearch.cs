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

        [Theory]
        [InlineData("4baf35a6982a81402fbe5882a47a75add97a01cc69fc418b5fc545026751f08a")]
        [InlineData("4BAF35A6982A81402FBE5882A47A75ADD97A01CC69FC418B5FC545026751F08A")]
        [InlineData("0x010a8144e644ace657426f10d35a2a3bee1d098e-0-0xdec45f94c65a79278b7a779a0f5e09a361672e5d-0x4baf35a6982a81402fbe5882a47a75add97a01cc69fc418b5fc545026751f08a-0")]
        public async void SearchNFT(string nftid)
        {
            var searchResult = await service.Search(nftid);
            Assert.NotEmpty(searchResult);
            Assert.IsType<NonFungibleToken>(searchResult![0]);
            Assert.Contains(nftid.ToLower(), (searchResult![0] as NonFungibleToken)!.id);
        }
    }
}
