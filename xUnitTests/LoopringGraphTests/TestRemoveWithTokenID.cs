using System;
using Lexplorer.Models;
using Lexplorer.Services;
using Xunit;

namespace xUnitTests.LoopringGraphTests
{
    [Collection("LoopringGraphQL collection")]
    public class TestRemoveWithTokenID
	{
        GraphQLTestsFixture fixture;
        LoopringGraphQLService service;

        public TestRemoveWithTokenID(GraphQLTestsFixture fixture)
        {
            this.fixture = fixture;
            service = fixture!.LGS;
        }

        [Theory]
        [InlineData("217")]
        public async void GetRemoveWithTokenID(string tokenID)
        {
            var remove = await service.GetAnyRemoveWithTokenID(tokenID);
            Assert.NotNull(remove);
            Assert.NotNull(remove!.pool);
            Assert.NotNull(remove!.token);
        }
	}
}

