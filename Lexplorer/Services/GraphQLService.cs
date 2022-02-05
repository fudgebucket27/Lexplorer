using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Lexplorer.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;

namespace Lexplorer.Services
{
    public class GraphQLService
    {
        const string _baseUrl = "https://api.thegraph.com/subgraphs/name/loopring/loopring";

        readonly RestClient _client;

        public GraphQLService()
        {
            _client = new RestClient(_baseUrl);
        }

        public async Task<BlockData> GetBlocks()
        {
            var blockQuery = @"
            query blocks(
                $skip: Int
                $first: Int
                $orderBy: Block_orderBy
                $orderDirection: OrderDirection
              ) {
                proxy(id: 0) {
                  blockCount
                  userCount
                }
                blocks(
                  skip: $skip
                  first: $first
                  orderBy: $orderBy
                  orderDirection: $orderDirection
                ) {
                  ...BlockFragment
                  transactionCount
                }
              }
            fragment BlockFragment on Block {
                id
                timestamp
                txHash
                gasLimit
                gasPrice
                height
                blockHash
                blockSize
                gasPrice
                operatorAccount {
                  ...AccountFragment
                }
              }
            fragment AccountFragment on Account {
                id
                address
            }
            ";

            var request = new RestRequest()
            {
                Method = Method.Post
            };
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = blockQuery,
                variables = new
                {
                    skip = 10,
                    first = 10,
                    orderBy = "internalID",
                    orderDirection = "desc"
                }
            });
            var response = await _client.PostAsync(request);
            var data = JsonConvert.DeserializeObject<BlockData>(response.Content);
            return data;
        }
    }
}
