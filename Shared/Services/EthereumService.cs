using Nethereum.Web3;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Lexplorer.Services
{
    public class EthereumService
    {
        public async Task<string?> GetMetadataLink(string? tokenId)
        {
            if (tokenId == null) return null;

            var web3 = new Web3($"https://mainnet.infura.io/v3/53173af3389645d18c3bcac2ee9a751c");
            var contractABI = "function uri(uint256 id) public returns (string memory)";
            try
            {
                var contract = web3.Eth.GetContract(contractABI, "0xB25f6D711aEbf954fb0265A3b29F7b9Beba7E55d");
                var function = contract.GetFunction("uri");
                object[] parameters = new object[1] { tokenId };
                var uri = await function.CallAsync<string>(parameters);
                return uri.Remove(0, 7); //remove the ipfs portion
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.StackTrace + "\n" + e.Message);
                return null;
            }
        }
    }
}
