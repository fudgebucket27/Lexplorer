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
            GraphQLRequest fetchBlocksGraphlQLQuery = new GraphQLHttpRequest
            {
                Query = @"
             query blocks(
                $skip: Int
                $first: Int
                $orderBy: Block_orderBy
                $orderDirection: OrderDirection
              ) {
                proxy(id: 0) {
                  blockCount
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
            ",
                Variables = new
                {
                    skip = 0,
                    first = 10,
                    orderBy = "internalID",
                    orderDirection = "desc"
                }
            };
            var response = await _client.SendQueryAsync<BlockData>(fetchBlocksGraphlQLQuery);
            return response;
        }
    }
}
