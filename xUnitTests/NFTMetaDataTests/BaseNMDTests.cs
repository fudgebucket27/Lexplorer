using System;
using Lexplorer.Services;
using Xunit;

namespace xUnitTests.NFTMetaDataTests
{
    //shared context for several xUnit test classes
    //https://xunit.net/docs/shared-context
    public class NFTMetaDataTestsFixture : IDisposable
    {
        public NftMetadataService NMS { get; private set; }
        public EthereumService EthS { get; private set; }

        public NFTMetaDataTestsFixture()
        {
            NMS = new NftMetadataService("https://loopring.mypinata.cloud/ipfs/");
            EthS = new EthereumService();
        }

        public void Dispose()
        {
            NMS.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    [CollectionDefinition("NFTMetaDataTests collection")]
    public class LoopringGraphQLCollection : ICollectionFixture<NFTMetaDataTestsFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}

