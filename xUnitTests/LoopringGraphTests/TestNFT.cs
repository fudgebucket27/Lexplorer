using System;
using Xunit;
using Lexplorer.Services;
using Lexplorer.Models;
using System.Collections.Generic;
using System.Linq;

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
            Assert.NotEqual(0, nft?.mintedAtTransaction?.amount);
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
        [InlineData("43569", 10)]
        public async void GetTotalNFTs(string accountID, int chunkSize = 1000)
        {
            var nftCount = await service.GetAccountTotalNfts(accountID, chunkSize);

            var nfts = await service.GetAccountNFTs(0, nftCount + 1, accountID);
            Assert.NotNull(nfts);

            Assert.Equal(nftCount, nfts!.Count);
        }

        [Theory]
        [InlineData("0x9af1b4f94657c79c4cff77c3c35a746353518724")]
        public async void GetCollectionNFTs(string tokenAddress)
        {
            var nfts = await service.GetCollectionNFTs(tokenAddress);
            Assert.NotEmpty(nfts);
        }

        [Fact]
        public async void GetNftHolders_WhenPassingValidNftId_ExpectListOfHolders()
        {
            string nftId = "0xf5ca5e167819d449270dae6fa22208085a2c03d9-0-0xcd56d4e1dba6edf425aa3e0f901d9f1645887539-0x33f5b90cdf6271026f4a0e4817de618268e6e391320a4ad834ea54fd87032fcb-2";
            List<NftHolder> holders = await service.GetNftHolders(nftId);
            Assert.NotEmpty(holders);

            holders.ForEach(x => Assert.True(x.Balance >= 1));
        }
    }
}
