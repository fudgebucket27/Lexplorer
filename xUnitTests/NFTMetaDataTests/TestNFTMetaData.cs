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
			Assert.Null(meta!.Error);
        }

        [Theory]
        [InlineData("ipfs://QmRJEhmpwKEn8U6NsHfqGt4ZXZKZm3vFpgRn269d5WsA5y", "video/mp4")] //0x4de8f2002b80be98ccab8746c6569850a36b9f5de85b2900f846fa6134bfc8b7
        [InlineData("ipfs://QmXo39B4QLDjaaGBNVkQVTqKQPuUvr1dz9DV48n94c5VJm", "image/jpeg")] //0x2b5c4503e39e88154bcafe015fafbaf61955a88d3e65ab2f3aad28e37124c74c
        [InlineData("ipfs://bafybeigbqythqw23mn3lugl7ae4nnoab2zkevapiff5lxx5hb2hngfhpwi", "audio/mpeg")] //0x52ed914d080ee393a35b02cc9e57f27fa96cc9ab933ee754b05ab61d49539546
        [InlineData("ipfs://QmdsJy2BehwHfMw34XneTmcmMAJin59uv9Lmw2tFCNKVin/3d.glb", "application/octet-stream", "model/gltf-binary")] //0xf11780791dfef9ca79a07f046e98ef0efdebecfaa763b24eb61ccaaca3132d32
        [InlineData("ipfs://QmYixrWjyLXEuaNsovWYW6tsrH3NVjRwJ7kUsTPGqZWKvS", "text/html")] //0x574e9ca4605e4ebff1d4e9b204b16fe73f122f82c60da0186af3ded68bff9c10
        public async void TestGetNFTContentType(string nftURL, params string[] contentTypes)
        {
            var contentType = await fixture.NMS.GetContentTypeFromURL(nftURL);
            Assert.NotNull(contentType);
            Assert.Contains(contentType, new List<string>(contentTypes));
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
                "{\"name\":\"Test\",\"properties\":[{\"key\": \"test\", \"value\": \"value\"}, {\"invalid\": \"value\"}]}");
            Assert.NotNull(meta);
            Assert.Equal("Test", meta!.name);
            Assert.Equal("value", meta!.properties!["test"]);
        }

        [Theory]
        [InlineData("ipfs://QmPbU7P8DmsGAspVrc4hdPXF5Z6P3NTXZfZJ9Q8sBmax9s/AOJETFinal#1.glb", "QmPbU7P8DmsGAspVrc4hdPXF5Z6P3NTXZfZJ9Q8sBmax9s/AOJETFinal%231.glb")]
        [InlineData("ipfs://ipfs/QmWLmY3Vif95cvMGNkkJDNJjyq7Z8YFLD8ngfuPs89SvWn", "QmWLmY3Vif95cvMGNkkJDNJjyq7Z8YFLD8ngfuPs89SvWn")]
        [InlineData("ipfs://QmT4enyxCxNytCcby23K8vtBhwteJVy7EJ1KjJhYaEVvhZ/Jolly Roger %230865.mp4", "QmT4enyxCxNytCcby23K8vtBhwteJVy7EJ1KjJhYaEVvhZ/Jolly%20Roger%20%230865.mp4")]
        public void MakeIPFSLink(string IPFSUrl, string realtivePinataURL)
        {
			var url = fixture.NMS.MakeIPFSLink(IPFSUrl);
            Assert.Equal(fixture.NMS.IPFSBaseUrl + realtivePinataURL, url);
        }
    }
}

