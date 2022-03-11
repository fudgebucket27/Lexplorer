using Lexplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Lexplorer.Services;

namespace xUnitTests.LoopringGraphTests
{
    [Collection("LoopringGraphQL collection")]
    public class TestAccount
    {
        GraphQLTestsFixture fixture;
        LoopringGraphQLService service;

        public TestAccount(GraphQLTestsFixture fixture)
        {
            this.fixture = fixture;
            this.service = fixture!.LGS;
        }

        [Fact]
        [TestPriority(-10)]
        public async void GetAccount()
        {
            Account? account = await service.GetAccount(fixture.testAccountId);
            Assert.NotNull(account);
            Assert.Equal(fixture.testAccountId, account!.id);
            Assert.Null(account.balances);
        }

        [Fact]
        public async void GetAccountBalances()
        {
            List<AccountTokenBalance>? balances = await service.GetAccountBalance(fixture.testAccountId);

            Assert.NotEmpty(balances);

            Assert.Null(balances![0].account);
        }
    }
}
