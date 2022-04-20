using System;
using Xunit;
using Lexplorer.Models;
using Lexplorer.Services;

namespace xUnitTests.LoopringGraphTests
{
    [Collection("LoopringGraphQL collection")]
    public class TestTransaction
    {
        GraphQLTestsFixture fixture;
        LoopringGraphQLService service;

        public TestTransaction(GraphQLTestsFixture fixture)
        {
            this.fixture = fixture;
            this.service = fixture!.LGS;
        }

        [Fact]
        public async void TestGetTransaction()
        {
            const string testID = "17878-249";
            Transaction? transaction = await service.GetTransaction(testID);
            Assert.NotNull(transaction);
            Assert.Equal(testID, transaction!.id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("Swap")]
        public async void TestGetTransations(string? typeName)
        {
            var transactions = await service.GetTransactions(0, 5, typeName);
            Assert.NotNull(transactions);
            Assert.NotEmpty(transactions?.data?.transactions);
            if (!String.IsNullOrEmpty(typeName))
            {
                foreach (var transaction in transactions?.data?.transactions!)
                {
                    Assert.Equal(typeName, transaction.typeName);
                }
            }
        }
    }
}

