using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
using Lexplorer.Models;
using System.Threading.Tasks;

namespace Lexplorer.Services
{
    public class NftMetadataService : IDisposable
    {
        const string BaseUrl = "https://fudgey.mypinata.cloud/ipfs/";

        readonly RestClient _client;

        public NftMetadataService()
        {
            _client = new RestClient(BaseUrl);
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<NftMetadata?> GetMetadata(string link)
        {
            //there is a fallback for if the metadata fails on the first try because
            //loopring deployed two different contracts for the nfts so some
            //metadata.json needs to be referenced directly while others are in a folder in ipfs
            NftMetadata? nmd = await GetMetadataFromURL(link);
            if (nmd == null)
                nmd = await GetMetadataFromURL(link + "/metadata.json");
            return nmd;
        }

        private async Task<NftMetadata?> GetMetadataFromURL(string URL)
        {
            var request = new RestRequest(URL);
            try
            {
                request.Timeout = 5000; //we can't afford to wait forever here, 1s must be enough
                var response = await _client.GetAsync(request);
                return JsonConvert.DeserializeObject<NftMetadata>(response.Content!);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.StackTrace + "\n" + e.Message);
                return null;
            }
        }

    }
}
