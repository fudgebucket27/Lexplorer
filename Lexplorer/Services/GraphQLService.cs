using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Lexplorer.Helpers;
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

        public async Task<Blocks> GetBlocks(int skip, int first)
        {
            var blocksQuery = @"
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
            "
            + GraphQLFragments.BlockFragment
            + GraphQLFragments.AccountFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = blocksQuery,
                variables = new
                {
                    skip = skip,
                    first = first,
                    orderBy = "internalID",
                    orderDirection = "desc"
                }
            });
            var response = await _client.PostAsync(request);
            var data = JsonConvert.DeserializeObject<Blocks>(response.Content);
            return data;
        }

        public async Task<Block> GetBlockDetails(int blockId)
        {
            var blockQuery = @"
            query block($id: ID!) {
                proxy(id: 0) {
                  blockCount
                }
                block(id: $id) {
                  ...BlockFragment
                  data
                  transactionCount
                  depositCount
                  withdrawalCount
                  transferCount
                  addCount
                  removeCount
                  orderbookTradeCount
                  swapCount
                  accountUpdateCount
                  ammUpdateCount
                  signatureVerificationCount
                  tradeNFTCount
                  swapNFTCount
                  withdrawalNFTCount
                  transferNFTCount
                  nftMintCount
                  nftDataCount
                }
              }
            "
            + GraphQLFragments.BlockFragment
            + GraphQLFragments.AccountFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = blockQuery,
                variables = new
                {
                    id = blockId
                }
            });
            var response = await _client.PostAsync(request);
            var data = JsonConvert.DeserializeObject<Block>(response.Content);
            return data;
        }

        public async Task<Swap> GetSwapTransaction(string transactionId)
        {
            var transactionQuery = @"
              query transaction($id: ID!) {
                transaction(id: $id) {
                  id
                  block {
                    id
                    blockHash
                    timestamp
                    transactionCount
                  }
                  data
                  ...AddFragment
                  ...RemoveFragment
                  ...SwapFragment
                  ...OrderbookTradeFragment
                  ...DepositFragment
                  ...WithdrawalFragment
                  ...TransferFragment
                  ...AccountUpdateFragment
                  ...AmmUpdateFragment
                  ...SignatureVerificationFragment
                  ...TradeNFTFragment
                  ...SwapNFTFragment
                  ...WithdrawalNFTFragment
                  ...TransferNFTFragment
                  ...MintNFTFragment
                  ...DataNFTFragment
                }
              }
            "
              + GraphQLFragments.AccountFragment
              + GraphQLFragments.TokenFragment
              + GraphQLFragments.PoolFragment
              + GraphQLFragments.NFTFragment
              + GraphQLFragments.AddFragment
              + GraphQLFragments.RemoveFragment
              + GraphQLFragments.SwapFragment
              + GraphQLFragments.OrderBookTradeFragment
              + GraphQLFragments.DepositFragment
              + GraphQLFragments.WithdrawalFragment
              + GraphQLFragments.TransferFragment
              + GraphQLFragments.AccountUpdateFragment
              + GraphQLFragments.AmmUpdateFragment
              + GraphQLFragments.SignatureVerificationFragment
              + GraphQLFragments.TradeNFTFragment
              + GraphQLFragments.SwapNFTFragment
              + GraphQLFragments.WithdrawalNFTFragment
              + GraphQLFragments.TransferNFTFragment
              + GraphQLFragments.MintNFTFragment
              + GraphQLFragments.DataNFTFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(new
            {
                query = transactionQuery,
                variables = new
                {
                    id = transactionId
                }
            });

            var response = await _client.PostAsync(request);
            var data = JsonConvert.DeserializeObject<Swap>(response.Content);
            return data;
        }

        public async Task<Transactions> GetTransactionsForBlock(int skip, int first, string? blockId = null, string? typeName = null)
        {
            var transactionsQuery = @"
              query transactions(
                $skip: Int
                $first: Int
                $orderBy: Transaction_orderBy
                $orderDirection: OrderDirection
                $block: Block_height
                $where: Transaction_filter
              ) {
                proxy(id: 0) {
                  transactionCount
                  depositCount
                  withdrawalCount
                  transferCount
                  addCount
                  removeCount
                  orderbookTradeCount
                  swapCount
                  accountUpdateCount
                  ammUpdateCount
                  signatureVerificationCount
                  tradeNFTCount
                  swapNFTCount
                  withdrawalNFTCount
                  transferNFTCount
                  nftMintCount
                  nftDataCount
                }
                transactions(
                  skip: $skip
                  first: $first
                  orderBy: $orderBy
                  orderDirection: $orderDirection
                  block: $block
                  where: $where
                ) {
                  id
                  internalID
                  block {
                    id
                    blockHash
                    timestamp
                    transactionCount
                    depositCount
                    withdrawalCount
                    transferCount
                    addCount
                    removeCount
                    orderbookTradeCount
                    swapCount
                    accountUpdateCount
                    ammUpdateCount
                    signatureVerificationCount
                    tradeNFTCount
                    swapNFTCount
                    withdrawalNFTCount
                    transferNFTCount
                    nftMintCount
                    nftDataCount
                  }
                  data
                  ...AddFragment
                  ...RemoveFragment
                  ...SwapFragment
                  ...OrderbookTradeFragment
                  ...DepositFragment
                  ...WithdrawalFragment
                  ...TransferFragment
                  ...AccountUpdateFragment
                  ...AmmUpdateFragment
                  ...SignatureVerificationFragment
                  ...TradeNFTFragment
                  ...SwapNFTFragment
                  ...WithdrawalNFTFragment
                  ...TransferNFTFragment
                  ...MintNFTFragment
                  ...DataNFTFragment
                }
              }"
              + GraphQLFragments.AccountFragment
              + GraphQLFragments.TokenFragment
              + GraphQLFragments.PoolFragment
              + GraphQLFragments.NFTFragment
              + GraphQLFragments.AddFragment
              + GraphQLFragments.RemoveFragment
              + GraphQLFragments.SwapFragment
              + GraphQLFragments.OrderBookTradeFragment
              + GraphQLFragments.DepositFragment
              + GraphQLFragments.WithdrawalFragment
              + GraphQLFragments.TransferFragment
              + GraphQLFragments.AccountUpdateFragment
              + GraphQLFragments.AmmUpdateFragment
              + GraphQLFragments.SignatureVerificationFragment
              + GraphQLFragments.TradeNFTFragment
              + GraphQLFragments.SwapNFTFragment
              + GraphQLFragments.WithdrawalNFTFragment
              + GraphQLFragments.TransferNFTFragment
              + GraphQLFragments.MintNFTFragment
              + GraphQLFragments.DataNFTFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            if (blockId != null)
            {
                request.AddJsonBody(new
                {
                    query = transactionsQuery,
                    variables = new
                    {
                        skip = skip,
                        first = first,
                        orderBy = "internalID",
                        orderDirection = "desc"
                    },
                    where = new
                    {
                        block = blockId
                    }
                });
            }
            else
            {
                request.AddJsonBody(new
                {
                    query = transactionsQuery,
                    variables = new
                    {
                        skip = skip,
                        first = first,
                        orderBy = "internalID",
                        orderDirection = "desc"
                    }
                });
            }

            var response = await _client.PostAsync(request);
            var data = JsonConvert.DeserializeObject<Transactions>(response.Content);
            return data;
        }
    }
}
