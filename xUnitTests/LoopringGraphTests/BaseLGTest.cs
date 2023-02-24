using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Lexplorer.Services;

namespace xUnitTests.LoopringGraphTests
{
    //shared context for several xUnit test classes
    //https://xunit.net/docs/shared-context
    public class GraphQLTestsFixture : IDisposable
    {
        public LoopringGraphQLService LGS { get; private set; }
        public string testAccountId { get; } = "12383";

        public GraphQLTestsFixture()
        {
            LGS = new LoopringGraphQLService("https://api.thegraph.com/subgraphs/name/loopring/loopring");
        }

        public void Dispose()
        {
            LGS.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    [CollectionDefinition("LoopringGraphQL collection")]
    public class LoopringGraphQLCollection : ICollectionFixture<GraphQLTestsFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

}
