using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace xUnitTests.ENSTests
{
    [Collection("ENSTests collection")]
    public class TestENSService
    {
        readonly ENSTestsFixture fixture;

        public TestENSService(ENSTestsFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("0x36cd6b3b9329c04df55d55d41c257a5fdd387acd", "0x99fdddfdc9277404db0379009274cc98d3688f8b")]
        public async void TestReverseLoopkup(params string[] addresses)
        {
            var domains = await fixture.ENS.ReverseLookup(addresses);
            Assert.NotEmpty(domains);
            Assert.Equal(addresses.Length, domains!.Count);
        }

        [Theory]
        [InlineData("0xaf0c3945c94f4271ded7bcdbf8762039cc36396a", "shortdestroyers.eth", "[0dcd3103d2187321948875f4b46de67bbcd107f64c1b76c1d3b6df44b2b178d7].loopring.eth")]
        [InlineData("0xabcdef0123543451231231324235432423423423")]
        public async void TestReverseLookupAddress(string address, params string[] domains)
        {
            var ens = await fixture.ENS.ReverseLookupAddress(address);
            var ensCount = (ens?.Count ?? 0);
            Assert.True(ensCount >= domains.Length, $"Should at least return {domains.Length} domains, but only returned {ensCount}");
            var ensArray = ens?.Keys.ToArray();
            foreach (var domain in domains)
            {
                Assert.True(ensArray!.Contains(domain), $"Domain {domain} not found in returned ensArray {ensArray}");
            }
        }

	}
}
