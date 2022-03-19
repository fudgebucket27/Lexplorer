using System;
using Xunit;
using Lexplorer.Services;

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
		[InlineData("0x4baf35a6982a81402fbe5882a47a75add97a01cc69fc418b5fc545026751f08a")]
		[InlineData("0x78cc3ebffd8628722aaf29681b45d6a342e4ae11520c1d507894cc0c86049075")]
		//[InlineData("0x01346618000000000000000002386f26fc10000000000000000000000000028d")] //loophead #653
		public async void TestGetMetadata(string nftID)
        {
			var link = await fixture.EthS.GetMetadataLink(nftID);
			Assert.NotNull(link);

			var meta = await fixture.NMS.GetMetadata(link!);
			Assert.NotNull(meta);
        }
	}
}

