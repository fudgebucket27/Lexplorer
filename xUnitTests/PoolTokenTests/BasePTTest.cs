using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Lexplorer.Services;

namespace xUnitTests.PoolTokenTests
{
    //shared context for several xUnit test classes
    //https://xunit.net/docs/shared-context
    public class PoolTokensTestsFixture : IDisposable
    {
        public LoopringGraphQLService LGS { get; private set; }
        public EthereumService ES { get; private set; }
        public LoopringPoolTokenCacheService LPTCS { get; private set; }

        public PoolTokensTestsFixture()
        {
            LGS = new LoopringGraphQLService("https://api.thegraph.com/subgraphs/name/juanmardefago/loopring36");
            ES = new EthereumService();
            LPTCS = new LoopringPoolTokenCacheService(LGS, ES);
            LPTCS.DisableCache();
        }

        public void Dispose()
        {
            LGS.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    [CollectionDefinition("PoolTokens collection")]
    public class PoolTokensCollection : ICollectionFixture<PoolTokensTestsFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

}
