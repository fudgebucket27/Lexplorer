using Lexplorer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTests.LoopringGraphTests;

[Collection("LoopringGraphQL collection")]
public class TestGetWhales
{
    GraphQLTestsFixture fixture;
    LoopringGraphQLService service;

    public TestGetWhales(GraphQLTestsFixture fixture)
    {
        this.fixture = fixture;
        service = fixture!.LGS;
    }

    [Fact]
    public async void GetWhales()
    {
        var whales = await service.GetWhales("1", first: 25);
        Assert.NotEmpty(whales);
        Assert.Equal(25, whales!.Count);
        whales.ForEach(x => Assert.NotNull(x.balance));
    }
}
