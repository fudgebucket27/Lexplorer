using System;
using Xunit;
using Lexplorer.Services;
using Lexplorer.Models;

namespace xUnitTests.LoopringGraphTests
{
    [Collection("LoopringGraphQL collection")]
    public class TestNFT
    {
        GraphQLTestsFixture fixture;
        LoopringGraphQLService service;

        private const string nftID = "0x010a8144e644ace657426f10d35a2a3bee1d098e-0-0xdec45f94c65a79278b7a779a0f5e09a361672e5d-0x4baf35a6982a81402fbe5882a47a75add97a01cc69fc418b5fc545026751f08a-0";

        public TestNFT(GraphQLTestsFixture fixture)
        {
            this.fixture = fixture;
            service = fixture!.LGS;
        }

        [Fact]
        [TestPriority(-10)]
        public async void GetNFTS()
        {
            var nfts = await service.GetNFTs(0, 10);
            Assert.NotNull(nfts);
            Assert.Equal(10, nfts?.Count);
            Assert.NotNull(nfts?[0]?.mintedAtTransaction);
            Assert.NotNull(nfts?[0]?.mintedAtTransaction?.id);
        }

        [Fact]
        public async void GetNFT()
        {
            var nft = await service.GetNFT(nftID);
            Assert.NotNull(nft);
            Assert.NotNull(nft?.mintedAtTransaction);
        }

        [Fact]
        public async void GetNFTTransactions()
        {
            var transactions = await service.GetNFTTransactions(0, 10, nftID);
            Assert.NotEmpty(transactions);
        }

        [Theory]
        [InlineData("100007")]
        [InlineData("10007")]
        public async void GetTotalNFTs(string accountID)
        {
            var nftCount = await service.GetAccountTotalNfts(accountID);

            var nfts = await service.GetAccountNFTs(0, nftCount + 1, accountID);
            Assert.NotNull(nfts);

            Assert.Equal(nftCount, nfts!.Count);
        }
    }
}
