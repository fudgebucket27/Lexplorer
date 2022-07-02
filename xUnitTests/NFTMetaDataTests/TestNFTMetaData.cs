using System;
using Xunit;
using Lexplorer.Services;
using System.Collections.Generic;

namespace xUnitTests.NFTMetaDataTests
{
	[Collection("NFTMetaDataTests collection")]
	public class TestNFTMetaData
	{
		readonly NFTMetaDataTestsFixture fixture;

		public TestNFTMetaData(NFTMetaDataTestsFixture fixture)
		{
			this.fixture = fixture;
		}

		[Theory]
		[InlineData("0x4baf35a6982a81402fbe5882a47a75add97a01cc69fc418b5fc545026751f08a", EthereumService.CF_NFTTokenAddress, 0)]
		[InlineData("0x78cc3ebffd8628722aaf29681b45d6a342e4ae11520c1d507894cc0c86049075", EthereumService.CF_NFTTokenAddress, 0)]
		[InlineData("0x01346618000000000000000002386f26fc1000000000000000000000000003a1", "0x1cacc96e5f01e2849e6036f25531a9a064d2fb5f", 0)] //loophead #929
		[InlineData("0x01346618000000000000000002386f26fc10000000000000000000000000028d", "0x1cacc96e5f01e2849e6036f25531a9a064d2fb5f", 0)] //loophead #653
		[InlineData("0x0000000000000000000000000000000000000000000000000000000000000006", "0x6a7ab7711adcfe67141df82ae853787ca93a7797", 0)] //metadata on arweave

		public async void TestGetMetadata(string nftID, string nftTokenAddress, int nftType)
        {
			var link = await fixture.EthS.GetMetadataLink(nftID, nftTokenAddress, nftType);
			Assert.NotNull(link);

			var meta = await fixture.NMS.GetMetadata(link!);
			Assert.NotNull(meta);
        }

		[Theory]
		[InlineData("0x4de8f2002b80be98ccab8746c6569850a36b9f5de85b2900f846fa6134bfc8b7", EthereumService.CF_NFTTokenAddress, 0)]
		public async void TestGetMetadataVideoContentType(string nftID, string nftTokenAddress, int nftType)
		{
			var link = await fixture.EthS.GetMetadataLink(nftID, nftTokenAddress, nftType);
			Assert.NotNull(link);

			var meta = await fixture.NMS.GetMetadata(link!);
			Assert.NotNull(meta);

			var contentType = await fixture.NMS.GetContentTypeFromURL(meta!.animation_url!);
			Assert.Equal("video/mp4", contentType);
		}

		[Theory]
		[InlineData("0x2b5c4503e39e88154bcafe015fafbaf61955a88d3e65ab2f3aad28e37124c74c", EthereumService.CF_NFTTokenAddress, 0)]
		public async void TestGetMetadataImageContentType(string nftID, string nftTokenAddress, int nftType)
		{
			var link = await fixture.EthS.GetMetadataLink(nftID, nftTokenAddress, nftType);
			Assert.NotNull(link);

			var meta = await fixture.NMS.GetMetadata(link!);
			Assert.NotNull(meta);

			var contentType = await fixture.NMS.GetContentTypeFromURL(meta!.animation_url!);
			Assert.Equal("image/jpeg", contentType);
		}

		[Theory]
		[InlineData("0x11b2e47f2c1d8cd2e22da989f2f0dde99a8d0a22a42381cfce9a5020fe7bd413", EthereumService.CF_NFTTokenAddress, 0)]
		public async void TestGetMetadataAudioContentType(string nftID, string nftTokenAddress, int nftType)
		{
			var link = await fixture.EthS.GetMetadataLink(nftID, nftTokenAddress, nftType);
			Assert.NotNull(link);

			var meta = await fixture.NMS.GetMetadata(link!);
			Assert.NotNull(meta);

			var contentType = await fixture.NMS.GetContentTypeFromURL(meta!.animation_url!);
			Assert.Equal("audio/mpeg", contentType);
		}

		[Theory]
		[InlineData("0xf11780791dfef9ca79a07f046e98ef0efdebecfaa763b24eb61ccaaca3132d32", EthereumService.CF_NFTTokenAddress, 0)]
		public async void TestGetMetadataModelContentType(string nftID, string nftTokenAddress, int nftType)
		{
			var link = await fixture.EthS.GetMetadataLink(nftID, nftTokenAddress, nftType);
			Assert.NotNull(link);

			var meta = await fixture.NMS.GetMetadata(link!);
			Assert.NotNull(meta);


			var contentType = await fixture.NMS.GetContentTypeFromURL(meta!.animation_url!);
			Assert.True(new List<string> { "application/octet-stream", "model/gltf-binary" }.Contains(contentType!), $"unexpected contentType \"{contentType}\"");
		}

		[Theory]
		[InlineData("0x574e9ca4605e4ebff1d4e9b204b16fe73f122f82c60da0186af3ded68bff9c10", EthereumService.CF_NFTTokenAddress, 0)]
		public async void TestGetMetadataHtmlContentType(string nftID, string nftTokenAddress, int nftType)
		{
			var link = await fixture.EthS.GetMetadataLink(nftID, nftTokenAddress, nftType);
			Assert.NotNull(link);

			var meta = await fixture.NMS.GetMetadata(link!);
			Assert.NotNull(meta);

			var contentType = await fixture.NMS.GetContentTypeFromURL(meta!.animation_url!);
			Assert.Equal("text/html", contentType);
		}

        [Fact]
		public void TestCorrectJSONPropertiesDictionary()
        {
			var meta = fixture.NMS.GetMetadataFromResponse(
				"{\"name\":\"Test\",\"properties\":{\"test\": \"value\"}}");
			Assert.NotNull(meta);
			Assert.Equal("Test", meta!.name);
			Assert.Equal("value", meta!.properties!["test"]);
        }

        [Fact]
        public void TestCorrectJSONPropertiesKeyValuArray()
        {
            var meta = fixture.NMS.GetMetadataFromResponse(
                "{\"name\":\"Test\",\"properties\":[{\"key\": \"test\", \"value\": \"value\"}]}");
            Assert.NotNull(meta);
            Assert.Equal("Test", meta!.name);
			Assert.Null(meta!.properties);
        }
    }
}

