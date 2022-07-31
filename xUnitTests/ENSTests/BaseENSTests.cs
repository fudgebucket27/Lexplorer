using System;
using Lexplorer.Services;
using Xunit;

namespace xUnitTests.ENSTests
{
    //shared context for several xUnit test classes
    //https://xunit.net/docs/shared-context
    public class ENSTestsFixture
    {

        public ENSCacheService ENS { get; private set; }

        public ENSTestsFixture()
        {
            ENS = new ENSCacheService("https://api.thegraph.com/subgraphs/name/ensdomains/ens");
            ENS.DisableCache();
        }
    }

    [CollectionDefinition("ENSTests collection")]
    public class ENSCollection : ICollectionFixture<ENSTestsFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }        
	
}
