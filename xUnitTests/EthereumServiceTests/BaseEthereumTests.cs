using System;
using Lexplorer.Services;
using Xunit;

namespace xUnitTests.NFTMetaDataTests
{
    //shared context for several xUnit test classes
    //https://xunit.net/docs/shared-context
    public class EthereumTestsFixture
    {

        public EthereumService EthS { get; private set; }

        public EthereumTestsFixture()
        {
            EthS = new EthereumService();
        }
    }

    [CollectionDefinition("EthereumTests collection")]
    public class EthereumCollection : ICollectionFixture<EthereumTestsFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}

