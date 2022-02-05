using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Lexplorer.Models;
using System.Diagnostics;

namespace Lexplorer.Services
{
    public class GraphQLService
    {
        const string _baseUrl = "https://api.thegraph.com/subgraphs/name/loopring/loopring";

        readonly GraphQLHttpClient _client;

        public GraphQLService()
        {
            _client = new GraphQLHttpClient(_baseUrl, new NewtonsoftJsonSerializer());
        }

        public async Task<GraphQLResponse<BlockData>> GetBlocks()
        {
            var response = await _client.SendQueryAsync<BlockData>(GraphQLConstants.FetchBlocksGraphlQLQuery);
            return response;
        }
    }
}
