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
            // Do "global" initialization here; Only called once.
            LGS = new LoopringGraphQLService();
        }

        public void Dispose()
        {
            // Do "global" teardown here; Only called once.
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
