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
            var request = new RestRequest(link);
            try
            {
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
