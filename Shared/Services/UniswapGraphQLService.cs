using Lexplorer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

namespace Lexplorer.Services
{
    public class UniswapGraphQLService
    {
        const string _baseUrl = "https://api.thegraph.com/subgraphs/name/uniswap/uniswap-v2";

        readonly RestClient _client;

        public UniswapGraphQLService()
        {
            _client = new RestClient(_baseUrl);
        }

        public async Task<UniswapToken?> GetTokenPrice(string address)
        {
            var tokenDayDataQuery = @"
             query tokenDayDatas($address: String!) {
                tokenDayDatas(
                  orderBy: date
                  orderDirection: desc
                  where: { token: $address }
                  first: 1
                ) {
                  priceUSD
                }
              }
            ";

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = tokenDayDataQuery,
                variables = new
                {
                    address = address
                }
            });
            try
            {
                var response = await _client.PostAsync(request);
                var data = JsonConvert.DeserializeObject<UniswapToken>(response.Content!)!;
                data.address = address;
                return data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
