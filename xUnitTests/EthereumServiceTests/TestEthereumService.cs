using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTests.EthereumServiceTests
{
    [Collection("EthereumTests collection")]
    public class TestEthereumService
    {
        readonly EthereumTestsFixture fixture;

        public TestEthereumService(EthereumTestsFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("fudgey.loopring.eth")]
        [InlineData("fudgey.eth")]
        [InlineData("bubblecum.eth")]
        public async void TestGetHexAddressFromsEns(string ens)
        {
            var hexAddress = await fixture.EthS.GetEthAddressFromEns(ens);
            Assert.NotNull(hexAddress);
        }

        [Theory]
        [InlineData("0xbbbbca6a901c926f240b89eacb641d8aec7aeafd", "LoopringCoin V2", "LRC")]
        [InlineData("0x6b175474e89094c44da98b954eedeac495271d0f", "Dai Stablecoin", "DAI")]
        [InlineData("0xff20817765cb7f73d4bde2e66e067e58d11095c2", "Amp", "AMP")]

        //MKR token symbol/name is non-compliant with bytes32 return values, now supported with fallback
        [InlineData("0x9f8F72aA9304c8B593d555F12eF6589cC3A579A2", "Maker", "MKR")]

        //pool tokens not working on graph/lexplorer
        [InlineData("0x69a8bdEE1af2138C58B1261373B37071850689C0", "AMM-MKR-ETH", "LP-MKR-ETH")] //pool 57, token 154
        [InlineData("0xc418a3af58d7a1bad0b709fe58d0afddf64e178d", "AMM-NEC-ETH", "LP-NEC-ETH")] //token 172
        [InlineData("0xa573c5d473702286f0ac84592eda49ad799ebaa1", "AMM-USDT-ETH", "LP-USDT-ETH")] //pool #2 token 84
        [InlineData("0x6d537764355bc23d4eadba7829048dac8215a73c", "AMM-ETH-USDT", "LP-ETH-USDT")] //pool #3, token 85
        [InlineData("0x4f23ca1cc6253dc1ba69a07a892d68f3b777c407", "AMM-BADGER-ETH", "LP-BADGER-ETH")] //pool #84, token 193
        [InlineData("0x8572b8a876f47d70128c73bfca049ce00eb77563", "AMM-MASK-ETH", "LP-MASK-ETH")] //pool #86, token 195
        [InlineData("0x4facf65a157678e62f84389dd248d99f828403d6", "AMM-0xBTC-ETH", "LP-0xBTC-ETH")] //pool #96, token 214
        [InlineData("0x18a1a6f47fd92185b91edc322d1954349ad0b652", "AMM-ALCX-ETH", "LP-ALCX-ETH")] //pool #100, token 222
        [InlineData("0xa186e201225e468218d53f3f9b42012022d425f3", "AMM-renDOGE-ETH", "LP-renDOGE-ETH")] //pool #106, token 233
        [InlineData("0xb8108988406db7c4035bcfef2bd924a9810ae7e6", "AMM-BTC2XFLI-ETH", "LP-BTC2XFLI-ETH")] //pool #110 token 241
        [InlineData("0x8195be4e48d3a2f80692fe1dba9b23b8050fb1f9", "AMM-SENT-USDC", "LP-SENT-USDC")] //pool #114, token 246
        //0x8f871ac37fa7f575e9b8c285b38f0bf99d3c087f token 163, pool #66
        //0xa2acf6b0304a808147ee3b10601e452c3f1bfde7 token 152, pool #55
        //0xa738de0f4b1f52cc8410d6e49ab6ed1ca3fe1420 token 148, pool #51
        //0xfb64c2d72e1caa0286899be8e4f88266c4d8ab9f token 146, pool #50
        //0x8303f865a2a221c920e9fcbf2e84703991f16251 token 142, pool #46
        //0xba64cdf65aea36ff4a58dcf288f1a62923555795 token 135, pool #39
        //0x22844c482b0626ac09b5689b4d8e81fe6710f5f4 token 131, pool #35
        //0xe8ea36f850db564408e4165a92bccb4e6e5f5e20 token 129, pool #33
        //0x9387e06961988726dd0732b6930be1c0a5343901 token 128, pool #32

        public async void TestGetTokenInfo(string contract, string expectedName, string expectedSymbol)
        {
            var name = await fixture.EthS.GetTokenNameFromAddress(contract);
            Assert.Equal(expectedName, name);
            var symbol = await fixture.EthS.GetTokenSymbolFromAddress(contract);
            Assert.Equal(expectedSymbol, symbol);
        }
    }
}
