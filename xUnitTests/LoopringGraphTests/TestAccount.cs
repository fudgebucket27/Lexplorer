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

        [Fact]
        public async void GetAccountNFTSlots()
        {
            IList<AccountNFTSlot>? slots = await service.GetAccountNFTs(0, 10, "32933");
            Assert.NotEmpty(slots);
            foreach (var slot in slots!)
            {
                Assert.NotNull(slot.nft);
            }
        }

        [Fact]
        public async void GetAccounts()
        {
            IList<Account>? accounts = await service.GetAccounts(0, 10);
            Assert.NotEmpty(accounts);
            foreach (var account in accounts!)
            {
                Assert.NotNull(account);
            }
        }
        [Fact]
        public async void GetPools()
        {
            IList<Account>? accounts = await service.GetAccounts(0, 10, "Pool");
            Assert.NotEmpty(accounts);
            foreach (var account in accounts!)
            {
                Assert.IsType<Pool>(account);
            }
        }
    }
}
