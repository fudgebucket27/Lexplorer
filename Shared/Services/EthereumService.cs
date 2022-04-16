using Nethereum.Web3;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Nethereum.ENS;

namespace Lexplorer.Services
{

    public class EthereumService
    {
        public const string CF_NFTTokenAddress = "0xB25f6D711aEbf954fb0265A3b29F7b9Beba7E55d";

        public async Task<string?> GetMetadataLink(string? tokenId, string? tokenAddress, int? nftType)
        {
            if (tokenId == null) return null;
            //call erc1155 or erc 721 contract depending on type 
            string? metadataLink = nftType == 0
                ? await GetMetadataLink(tokenId, tokenAddress, "function uri(uint256 id) external view returns (string memory)", "uri") 
                : await GetMetadataLink(tokenId, tokenAddress, "function tokenURI(uint256 tokenId) public view virtual override returns (string memory)", "tokenURI");
            if (metadataLink == null)
                metadataLink = await GetMetadataLink(tokenId, CF_NFTTokenAddress, "function uri(uint256 id) external view returns (string memory)", "uri"); //call counterfactual nft contract

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
                return uri;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.StackTrace + "\n" + e.Message);
                return null;
            }
        }

        public async Task<string?> GetEthAddressFromEns(string? ens)
        {

            var web3 = new Web3("https://mainnet.infura.io/v3/53173af3389645d18c3bcac2ee9a751c");
            var ensService = new ENSService(web3);
           
            try
            {
                return await ensService.ResolveAddressAsync(ens);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.StackTrace + "\n" + e.Message);
                return null;
            }
        }
    }
}
