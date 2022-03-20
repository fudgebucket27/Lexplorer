using Nethereum.Web3;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Lexplorer.Services
{
    public class EthereumService
    {
        public async Task<string?> GetMetadataLink(string? tokenId, string? tokenAddress)
        {
            if (tokenId == null) return null;
            String? metadataLink = metadataLink = await GetMetadataLink(tokenId, tokenAddress, "function uri(uint256 id) external view returns (string memory)", "uri"); //call erc1155 contract 
            if (metadataLink == null)
                metadataLink = await GetMetadataLink(tokenId, tokenAddress, "function tokenURI(uint256 tokenId) public view virtual override returns (string memory)", "tokenURI"); //call erc721 nft contract 
            if (metadataLink == null)
                metadataLink = await GetMetadataLink(tokenId, "0xB25f6D711aEbf954fb0265A3b29F7b9Beba7E55d", "function uri(uint256 id) external view returns (string memory)", "uri"); //call counterfactual nft contract

            return metadataLink;
        }

        public async Task<string?> GetMetadataLink(string? tokenId, string? tokenAddress, string? contractABI, string? functionName)
        {

            var web3 = new Web3("https://mainnet.infura.io/v3/53173af3389645d18c3bcac2ee9a751c");
            try
            {
                var contract = web3.Eth.GetContract(contractABI, tokenAddress);
                var function = contract.GetFunction(functionName);
                object[] parameters = new object[1] { tokenId! };
                var uri = await function.CallAsync<string>(parameters);
                if (uri == null) return null;
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
