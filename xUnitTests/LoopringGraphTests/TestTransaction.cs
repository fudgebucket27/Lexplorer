﻿using System;
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
            service = fixture!.LGS;
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
            var transactions = await service.GetTransactions(0, 5, typeName: typeName);
            Assert.NotEmpty(transactions);
            if (!string.IsNullOrEmpty(typeName))
            {
                foreach (var transaction in transactions!)
                {
                    Assert.Equal(typeName, transaction.typeName);
                }
            }
        }

        [Theory]
        [InlineData("19527")]
        public async void TestGetBlockTransations(string? blockId)
        {
            var transactions = await service.GetTransactions(0, 5, blockId: blockId);
            Assert.NotEmpty(transactions);
        }
    }
}

