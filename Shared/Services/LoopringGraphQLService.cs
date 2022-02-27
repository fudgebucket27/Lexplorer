using Lexplorer.Helpers;
using Lexplorer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Lexplorer.Services
{
    public class LoopringGraphQLService
    {
        const string _baseUrl = "https://api.thegraph.com/subgraphs/name/loopring/loopring";

        readonly RestClient _client;

        public LoopringGraphQLService()
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
            var data = JsonConvert.DeserializeObject<Blocks>(response.Content!);
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
            var data = JsonConvert.DeserializeObject<Block>(response.Content!);
            return data;
        }

        public async Task<Transaction?> GetTransaction(string transactionId)
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
              + GraphQLFragments.AccountCreatedAtFragment
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

            try
            {
                var response = await _client.PostAsync(request);
                JObject jresponse = JObject.Parse(response.Content!);
                JToken result = jresponse["data"]!["transaction"]!;
                return result.ToObject<Transaction>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Transactions?> GetTransactions(int skip, int first, string? blockId = null, string? typeName = null)
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
              + GraphQLFragments.AccountCreatedAtFragment
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
                        ,
                        where = new
                        {
                            block = blockId
                        }
                    }
                });
            }
            else if(typeName != null)
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
                                       ,
                        where = new
                        {
                            typename = typeName
                        }
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

            try
            { 
                var response = await _client.PostAsync(request);
                var data = JsonConvert.DeserializeObject<Transactions>(response.Content!);
                return data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<Account?> GetAccount(string accountId)
        {
            var accountQuery = @"
            query account(
                $accountId: Int
              ) {
                account(
                  id: $accountId
                ) {
                  ...PoolFragment
                  ...UserFragment
                }
            }"
            + GraphQLFragments.PoolFragment
            + GraphQLFragments.UserFragment
            + GraphQLFragments.AccountCreatedAtFragment
            + GraphQLFragments.BlockFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = accountQuery,
                variables = new
                {
                    accountId = int.Parse(accountId)
                }
            });
            try
            {
                var response = await _client.PostAsync(request);
                JObject jresponse = JObject.Parse(response.Content!);
                JToken result = jresponse["data"]!["account"]!;
                return result.ToObject<Account>()!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<List<AccountBalance>?> GetAccountBalance(string accountId)
        {
            var balanceQuery = @"
            query accountBalances(
                $accountId: Int
              ) {
                account(
                  id: $accountId
                ) {
                  balances 
                  {
                    id
                    balance
                    token
                    {
                      ...TokenFragment
                    }
                  }
                }
             }
            "
              + GraphQLFragments.TokenFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = balanceQuery,
                variables = new
                {
                    accountId = Int32.Parse(accountId)
                }
            });
            var response = await _client.PostAsync(request);
            try
            {
                JObject jresponse = JObject.Parse(response.Content!);
                IList<JToken> balanceTokens = jresponse["data"]!["account"]!["balances"]!.Children().ToList();
                List<AccountBalance> balances = new List<AccountBalance>();
                foreach (JToken result in balanceTokens)
                {
                    // JToken.ToObject is a helper method that uses JsonSerializer internally
                    AccountBalance balance = result.ToObject<AccountBalance>()!;
                    balances.Add(balance);
                }
                return balances;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<string?> GetAccountTransactionsResponse(int skip, int first, string accountId,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            var accountQuery = @"
            query accountTransactions(
                $skip: Int
                $first: Int
                $accountId: Int
                $orderBy: Transaction_orderBy
                $orderDirection: OrderDirection
                $where: Transaction_filter
              ) {
                account(
                  id: $accountId
                ) {
                  transactions(
                    skip: $skip
                    first: $first
                    orderBy: $orderBy
                    orderDirection: $orderDirection
                  ) {
                    id
                    __typename
                    block (
                      where: $where
                    ) {
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
             }
            "
              + GraphQLFragments.AccountFragment
              + GraphQLFragments.TokenFragment
              + GraphQLFragments.PoolFragment
              + GraphQLFragments.AccountCreatedAtFragment
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
            if ((startDate != null) && (endDate != null))
            {
                Int64 startDateUx = ((DateTimeOffset)startDate).ToUnixTimeSeconds();
                Int64 endDateUx = ((DateTimeOffset)endDate).ToUnixTimeSeconds();
                request.AddJsonBody(new
                {
                    query = accountQuery,
                    variables = new
                    {
                        skip = skip,
                        first = first,
                        accountId = int.Parse(accountId),
                        orderBy = "internalID",
                        orderDirection = "desc",
                        where = new
                        { 
                            startDateUx = startDateUx, 
                            endDateUx = endDateUx 
                        },
                        //todo: add where but how?
                        //account.transaction.block.timestamp
                    }
                });
            }
            else
            {
                request.AddJsonBody(new
                {
                    query = accountQuery,
                    variables = new
                    {
                        skip = skip,
                        first = first,
                        accountId = int.Parse(accountId),
                        orderBy = "internalID",
                        orderDirection = "desc"
                    }
                });
            }
            try
            {
                var response = await _client.PostAsync(request);
                return response.Content;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IList<Transaction>?> GetAccountTransactions(int skip, int first, string accountId,
            DateTime? startDate = null, DateTime? endDate = null)
        { 
            try
            {
                string? response = await GetAccountTransactionsResponse(skip, first, accountId, startDate, endDate);
                JObject jresponse = JObject.Parse(response!);
                JToken? token = jresponse["data"]!["account"]!["transactions"];
                return token!.ToObject<IList<Transaction>>(); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Pairs?> GetPairs(int skip = 0, int first = 10 , string orderBy = "tradedVolumeToken0Swap", string orderDirection = "desc")
        {
            var pairsQuery = @"
             query pairs(
                $skip: Int
                $first: Int
                $orderBy: Pair_orderBy
                $orderDirection: OrderDirection
              ) {
                pairs(
                  skip: $skip
                  first: $first
                  orderBy: $orderBy
                  orderDirection: $orderDirection
                ) {
                  id
                  internalID
                  token0 {
                    ...TokenFragment
                  }
                  token1 {
                    ...TokenFragment
                  }
                  dailyEntities(skip: 1, first: 1, orderBy: dayEnd, orderDirection: desc) {
                    tradedVolumeToken1Swap
                    tradedVolumeToken0Swap
                    id
                  }
                  weeklyEntities(
                    skip: 0
                    first: 1
                    orderBy: weekEnd
                    orderDirection: desc
                  ) {
                    tradedVolumeToken1Swap
                    tradedVolumeToken0Swap
                    id
                  }
                }
              }
            "
              + GraphQLFragments.TokenFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = pairsQuery,
                variables = new
                {
                    skip = skip,
                    first = first,
                    orderBy = orderBy,
                    orderDirection = orderDirection
                }
            });
            try
            {
                var response = await _client.PostAsync(request);
                var data = JsonConvert.DeserializeObject<Pairs>(response.Content!);
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
