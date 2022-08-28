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
using RestSharp.Serializers;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Numerics;

namespace Lexplorer.Services
{
    public class LoopringGraphQLService : IDisposable
    {
        readonly RestClient _client;

        public LoopringGraphQLService(IConfiguration config)
        {
            _client = new RestClient(config.GetSection("services:loopringgraph:endpoint").Value);
        }

        public LoopringGraphQLService(string baseUrl)
        {
            _client = new RestClient(baseUrl);
        }

        public async Task<BlocksAndProxyDTO?> GetBlocks(int skip, int first, string orderBy = "internalID",
            string orderDirection = "desc", string? blockTimestamp = null, bool gte = true)
        {
            var blocksQuery = @"
            query blocks(
                $skip: Int
                $first: Int
                $orderBy: Block_orderBy
                $orderDirection: OrderDirection
                $where: Block_filter
              ) {
                proxy(id: 0) {
                  blockCount
                  userCount
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
                blocks(
                  skip: $skip
                  first: $first
                  orderBy: $orderBy
                  orderDirection: $orderDirection
                  where: $where
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
            JObject jObject = JObject.FromObject(new
            {
                query = blocksQuery,
                variables = new
                {
                    skip = skip,
                    first = first,
                    orderBy = orderBy,
                    orderDirection = orderDirection,
                    where = new
                    {
                    }
                }
            });
            if (blockTimestamp != null)
            {
                JObject where = (jObject["variables"]!["where"] as JObject)!;
                where.Add(gte
                    ? new JProperty("timestamp_gte", blockTimestamp)
                    : new JProperty("timestamp_lte", blockTimestamp));
            }
            request.AddStringBody(jObject.ToString(), ContentType.Json);
            try
            {
                var response = await _client.PostAsync(request);
                var jtoken = JToken.Parse(response.Content!);
                return jtoken["data"]!.ToObject<BlocksAndProxyDTO>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Tuple<double, double>?> GetBlockDateRange(DateTime startDateUTC, DateTime endDateUTC)
        {
            var blockQuery = @"
            query swap (
              $timeStampgte: BigInt
              $timeStamplte: BigInt
              )
              {
                firstblock:blocks (first: 1,
                  orderBy: internalID
                  orderDirection: asc
                  where: {
                    timestamp_gte: $timeStampgte
                })
                 {
                  id
                  timestamp
                  }
                lastblock:blocks (first: 1,
                  orderBy: internalID
                  orderDirection: desc
                  where: {
                    timestamp_lte: $timeStamplte
                })
                 {
                  id
                  timestamp
                  }
                }
            ";

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = blockQuery,
                variables = new
                {
                    timeStampgte = TimestampConverter.ToTimeStamp(startDateUTC),
                    timeStamplte = TimestampConverter.ToTimeStamp(endDateUTC)
                }
            });
            try
            {
                var response = await _client.PostAsync(request);
                JToken token = JToken.Parse(response.Content!);
                return new Tuple<double, double>(
                    (double)token["data"]!["firstblock"]![0]!["id"]!,
                    (double)token["data"]!["lastblock"]![0]!["id"]!);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<BlockAndProxyDTO?> GetBlockDetails(int blockId, CancellationToken cancellationToken = default)
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
            var response = await _client.PostAsync(request, cancellationToken);
            var jToken = JToken.Parse(response.Content!);
            return jToken["data"]!.ToObject<BlockAndProxyDTO>();
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

        public async Task<List<Transaction>?> GetTransactions(int skip, int first, string? blockId = null, string? typeName = null, CancellationToken cancellationToken = default)
        {
            var transactionsQuery = @"
              query transactions(
                $skip: Int
                $first: Int
                $orderBy: Transaction_orderBy
                $orderDirection: OrderDirection
                $whereFilter: Transaction_filter
              ) {
                transactions(
                  skip: $skip
                  first: $first
                  orderBy: $orderBy
                  orderDirection: $orderDirection
                  where: $whereFilter
                ) {
                  id
                  internalID
                  block {
                    timestamp
                  }
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
              + GraphQLTransactionListFragments.AllFragments;

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
                        orderDirection = "desc",
                        whereFilter = new
                        {
                            block = blockId
                        }
                    }
                });
            }
            else if (typeName != null)
            {
                request.AddJsonBody(new
                {
                    query = transactionsQuery,
                    variables = new
                    {
                        skip = skip,
                        first = first,
                        orderBy = "internalID",
                        orderDirection = "desc",
                        whereFilter = new
                        {
                            typename = typeName
                        }
                    }
                });
            }
            else
            {
                //remove unused parameter to fix "Invalid query" error with non-hosted graph
                //split and re-join all lines which don't contain the word "whereFilter"
                transactionsQuery = string.Join(Environment.NewLine,
                    transactionsQuery.Split(Environment.NewLine).Where(
                        (a) => !(a.Contains("whereFilter"))));
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
                var response = await _client.PostAsync(request, cancellationToken);
                var jtoken = JToken.Parse(response.Content!);
                return jtoken["data"]!["transactions"]!.ToObject<List<Transaction>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<IList<Account>?> GetAccounts(int skip, int first, string? typeName = null, CancellationToken cancellationToken = default)
        {
            var accountsQuery = @"
                      query accounts(
                        $skip: Int
                        $first: Int
                        $orderBy: Account_orderBy
                        $orderDirection: OrderDirection
                      ) {
                        accounts(
                          skip: $skip
                          first: $first
                          orderBy: $orderBy
                          orderDirection: $orderDirection
                        ) {
                          ...AccountFragment
                          ...PoolFragment
                          ...UserFragment
                        }
                      }"
                  + GraphQLFragments.AccountFragment
                  + GraphQLFragments.TokenFragment
                  + GraphQLFragments.PoolFragment
                  + GraphQLFragments.UserFragment
                  + GraphQLFragments.AccountCreatedAtFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");

            //since there is no way to filter for typename in Accounts_filter, we simply
            //query a different entity, i.e. pools instead of accounts
            string typePluralName = "accounts";
            if (!string.IsNullOrEmpty(typeName))
            {
                //lower case, but first char only
                typePluralName = char.ToLower(typeName[0]) + typeName.Substring(1) + "s";
                accountsQuery = accountsQuery.Replace("accounts(", $"{typePluralName}(");
            }

            request.AddJsonBody(new
            {
                query = accountsQuery,
                variables = new
                {
                    skip = skip,
                    first = first,
                    orderBy = "internalID",
                    orderDirection = "desc"
                }
            });

            try
            {
                var response = await _client.PostAsync(request, cancellationToken);
                JObject jresponse = JObject.Parse(response.Content!);
                JToken? token = jresponse["data"]![typePluralName];
                return token!.ToObject<IList<Account>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<Account?> GetAccount(string accountId, CancellationToken cancellationToken = default)
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
                var response = await _client.PostAsync(request, cancellationToken);
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
        public async Task<List<AccountTokenBalance>?> GetAccountBalance(string accountId, CancellationToken cancellationToken = default)
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
                    accountId = int.Parse(accountId)
                }
            });
            var response = await _client.PostAsync(request, cancellationToken);
            try
            {
                JObject jresponse = JObject.Parse(response.Content!);
                IList<JToken> balanceTokens = jresponse["data"]!["account"]!["balances"]!.Children().ToList();
                List<AccountTokenBalance> balances = new List<AccountTokenBalance>();
                foreach (JToken result in balanceTokens)
                {
                    // JToken.ToObject is a helper method that uses JsonSerializer internally
                    AccountTokenBalance balance = result.ToObject<AccountTokenBalance>()!;
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
            double? firstBlockId = null, double? lastBlockId = null, CancellationToken cancellationToken = default)
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
                    where: $where
                  ) {
                    id
                    __typename
                    block {
                      timestamp
                    }
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
              + GraphQLTransactionListFragments.AllFragments;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            JObject jObject = JObject.FromObject(new
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
                    },
                }
            });
            if (firstBlockId.HasValue && lastBlockId.HasValue)
            {
                JObject where = (jObject["variables"]!["where"] as JObject)!;
                where.Add(new JProperty("block_gte", firstBlockId.ToString()));
                where.Add(new JProperty("block_lte", lastBlockId.ToString()));
            }
            request.AddStringBody(jObject.ToString(), ContentType.Json);
            try
            {
                var response = await _client.PostAsync(request, cancellationToken);
                return response.Content;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IList<Transaction>?> GetAccountTransactions(int skip, int first, string accountId,
            double? firstBlockId = null, double? lastBlockId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                string? response = await GetAccountTransactionsResponse(skip, first, accountId, firstBlockId, lastBlockId, cancellationToken);
                JObject jresponse = JObject.Parse(response!);
                JToken? token = jresponse["data"]!["account"]!["transactions"];
                return token!.ToObject<IList<Transaction>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new List<Transaction>();
            }
        }

        public async Task<IList<AccountNFTSlot>?> GetAccountNFTs(int skip, int first, string accountId, CancellationToken cancellationToken = default)
        {
            var accountNFTQuery = @"
            query accountNFTSlotsQuery(
                $skip: Int
                $first: Int
                $orderBy: AccountNFTSlot_orderBy
                $orderDirection: OrderDirection
                $where: AccountNFTSlot_filter
              ) {
                accountNFTSlots(
                    skip: $skip
                    first: $first
                    orderBy: $orderBy
                    orderDirection: $orderDirection
                    where: $where
                  ) {
                    id
                    __typename
                    balance
                    nft {
                      id
                      ...NFTFragment
                    }
                }
             }
            "
              + GraphQLFragments.AccountFragment
              + GraphQLFragments.TokenFragment
              + GraphQLFragments.NFTFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            JObject jObject = JObject.FromObject(new
            {
                query = accountNFTQuery,
                variables = new
                {
                    skip = skip,
                    first = first,
                    orderBy = "lastUpdatedAt",
                    orderDirection = "desc",
                    where = new
                    {
                        account = accountId,
                        nft_not = ""
                    },
                }
            });
            request.AddStringBody(jObject.ToString(), ContentType.Json);
            try
            {
                var response = await _client.PostAsync(request, cancellationToken);
                JObject jresponse = JObject.Parse(response.Content!);
                JToken? token = jresponse["data"]!["accountNFTSlots"];
                return token!.ToObject<IList<AccountNFTSlot>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<int> GetAccountTotalNfts(string accountId, int chunkSize = 1000, CancellationToken cancellationToken = default)
        {
            var query = @"
                        query accountNFTSlotsQuery(
                        $skip: Int
                        $first: Int
                        $where: AccountNFTSlot_filter
                      ) {
                        accountNFTSlots(
                            skip: $skip
                            first: $first
                            where: $where
                          ) {
                            id
                        }
                     }
                    ";

            chunkSize = Math.Min(1000, chunkSize); //enforce max value of "first" from thegraph, i.e. 1000
            int count = 0;

            while (true)
            {
                try
                {
                    var request = new RestRequest();
                    request.AddHeader("Content-Type", "application/json");
                    JObject jObject = JObject.FromObject(new
                    {
                        query = query,
                        variables = new
                        {
                            skip = count,
                            first = chunkSize,
                            where = new
                            {
                                account = accountId,
                                nft_not = ""
                            },
                        }
                    });
                    request.AddStringBody(jObject.ToString(), ContentType.Json);

                    var response = await _client.PostAsync(request, cancellationToken);
                    JObject jresponse = JObject.Parse(response.Content!);
                    int chunkCount = jresponse["data"]!["accountNFTSlots"]!.Count();
                    count += chunkCount;

                    if (chunkCount < chunkSize)
                        break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    break;
                }
            }

            return count;
        }
        public async Task<List<Pair>?> GetPairs(int skip = 0, int first = 10, string orderBy = "tradedVolumeToken0Swap", string orderDirection = "desc")
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
                  dailyEntities(skip: 0, first: 1, orderBy: dayEnd, orderDirection: desc) {
                    id
                    tradedVolumeToken0
                    tradedVolumeToken1
                  }
                  weeklyEntities(
                    skip: 0
                    first: 1
                    orderBy: weekEnd
                    orderDirection: desc
                  ) {
                    id
                    tradedVolumeToken0
                    tradedVolumeToken1
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
                JObject jresponse = JObject.Parse(response.Content!);
                JToken? jtoken = jresponse["data"]!["pairs"];
                return jtoken!.ToObject<List<Pair>>()!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Pair?> GetPair(string pairID)
        {
            var pairQuery = @"
             query pair(
                $pairID: ID!
              ) {
                pair(
                  id: $pairID
                ) {
                  id
                  internalID
                  token0 {
                    ...TokenFragment
                  }
                  token1 {
                    ...TokenFragment
                  }
                  token0Price
                  token1Price
                  tradedVolumeToken0
                  tradedVolumeToken1
                  tradedVolumeToken0Swap
                  tradedVolumeToken1Swap
                  tradedVolumeToken0Orderbook
                  tradedVolumeToken1Orderbook
                }
              }
            "
              + GraphQLFragments.TokenFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = pairQuery,
                variables = new
                {
                    pairID
                }
            });
            try
            {
                var response = await _client.PostAsync(request);
                JObject jresponse = JObject.Parse(response.Content!);
                JToken? token = jresponse["data"]!["pair"];
                return token!.ToObject<Pair>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IList<PairDailyData>?> GetPairDailyEntities(string pairID, int skip, int first, string orderBy = "dayEnd", string orderDirection = "desc")
        {
            var pairDailyEntitiesQuery = @"
             query pair(
                $pairID: ID!
                $skip: Int
                $first: Int
                $orderBy: PairDailyData_orderBy
                $orderDirection: OrderDirection
              ) {
                pair(
                  id: $pairID
                ) {
                  dailyEntities(
                    skip: $skip
                    first: $first
                    orderBy: $orderBy
                    orderDirection: $orderDirection
                  ) {
                    id
                    dayStart
                    dayEnd
                    dayNumber
                    token0PriceLow
                    token0PriceHigh
                    token0PriceOpen
                    token0PriceClose

                    token1PriceLow
                    token1PriceHigh
                    token1PriceOpen
                    token1PriceClose

                    tradedVolumeToken0
                    tradedVolumeToken0Swap
                    tradedVolumeToken0Orderbook
                    tradedVolumeToken1
                    tradedVolumeToken1Swap
                    tradedVolumeToken1Orderbook
                  }
                }
              }
            "
              + GraphQLFragments.TokenFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = pairDailyEntitiesQuery,
                variables = new
                {
                    pairID,
                    skip,
                    first,
                    orderBy,
                    orderDirection
                }
            });
            try
            {
                var response = await _client.PostAsync(request);
                JObject jresponse = JObject.Parse(response.Content!);
                JToken? token = jresponse["data"]!["pair"]!["dailyEntities"];
                return token!.ToObject<IList<PairDailyData>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private static readonly Dictionary<string, Type> resultCategories
            = new Dictionary<string, Type>
        {
                { "accounts", typeof(Account) },
                { "accountsByAddress", typeof(Account) },
                { "blocks", typeof(Block) },
                { "transactions", typeof(Transaction) },
                { "nonFungibleTokens", typeof(NonFungibleToken) },
                { "nonFungibleTokensBynftID", typeof(NonFungibleToken) },
                { "tokens", typeof(Token) },
        };
        public async Task<IList<object>?> Search(string searchTerm)
        {
            var searchQuery = @"
             query search(
                $searchTerm: String
                $searchTermBytes: Bytes
              ) {
                accounts(
                  where: {id: $searchTerm}
                ) {
                  id
                  __typename
                }
                accountsByAddress: accounts(
                  where: {address: $searchTermBytes}
                ) {
                  id
                  address
                  __typename
                }
                blocks(
                  where: {id: $searchTerm}
                ) {
                  id
                  __typename
                }
                transactions(
                  where: {id: $searchTerm}
                ) {
                  id
                  __typename
                }
                nonFungibleTokens(
                  where: {id: $searchTerm}
                ) {
                  id
                  __typename
                  nftID
                }
                nonFungibleTokensBynftID: nonFungibleTokens(
                  where: {nftID: $searchTermBytes}
                ) {
                  id
                  __typename
                  nftID
                }
                tokens(where: {symbol_contains_nocase: $searchTerm}
                ) {
                  id
                  __typename
                  symbol
                  name
                }
              }
            ";

            string searchTermBytes = "";
            if (BigInteger.TryParse(searchTerm, out BigInteger nftBigIntID))
            {
                //strip of any leading zeros (#173) but also ensure even number of bytes (#183)
                searchTermBytes = nftBigIntID.ToString("X").ToLower().Trim('0');
                if (searchTermBytes.Length % 2 == 1)
                    searchTermBytes = "0x0" + searchTermBytes;
                else
                    searchTermBytes = "0x" + searchTermBytes;
            }
            else
            {
                //avoid query errors with search strings that cannot be converted to bytes
                //extra searchTermBytes is only filled if it matches strict RegEx, starting with 0x (added if missing)
                //and then any number of pairs "({2})+" of 0-9, a-f, A-F, end must be reached = $
                searchTermBytes = (searchTerm.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase) ? searchTerm : "0x" + searchTerm).ToLower();
                if (!Regex.Match(searchTermBytes, "0x([a-f0-9]{2})+$").Success)
                    searchTermBytes = "";
            }
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = searchQuery,
                variables = new
                {
                    searchTerm = searchTerm,
                    searchTermBytes = searchTermBytes
                }
            });
            try
            {
                var response = await _client.PostAsync(request);
                List<object> results = new List<object>();
                JObject responseData = JObject.Parse(response.Content!);
                foreach (var item in resultCategories)
                {
                    JToken? token = responseData["data"]?[item.Key];
                    if (token == null) continue;
                    IList<JToken> resultsPerCategory = token.Children().ToList();
                    foreach (JToken result in resultsPerCategory)
                    {
                        results.Add(result.ToObject(item.Value)!);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IList<NonFungibleToken>?> GetNFTs(int skip = 0, int first = 10, string orderBy = "mintedAt", string orderDirection = "desc")
        {
            var nonFungibleTokensQuery = @"
             query nonFungibleTokensQuery(
                $skip: Int
                $first: Int
                $orderBy: NonFungibleToken_orderBy
                $orderDirection: OrderDirection
              ) {
                nonFungibleTokens(
                  skip: $skip
                  first: $first
                  orderBy: $orderBy
                  orderDirection: $orderDirection
                ) {
                  ...NFTFragment
                  mintedAtTransaction {
                    ...MintNFTFragmentWithoutNFT
                  }
                }
              }
            "
              + GraphQLFragments.NFTFragment
              + GraphQLFragments.MintNFTFragmentWithoutNFT
              + GraphQLFragments.AccountFragment
              + GraphQLFragments.TokenFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = nonFungibleTokensQuery,
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
                JObject jresponse = JObject.Parse(response.Content!);
                JToken? token = jresponse["data"]!["nonFungibleTokens"];
                return token!.ToObject<IList<NonFungibleToken>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<NonFungibleToken?> GetNFT(string NFTId, CancellationToken cancellationToken = default)
        {
            var NFTQuery = @"
              query nonFungibleTokenQuery($id: ID!) {
                nonFungibleToken(id: $id) {
                  id
                  mintedAtTransaction {
                    ...MintNFTFragmentWithoutNFT
                  }
                  ...NFTFragment
                }
              }
            "
              + GraphQLFragments.AccountFragment
              + GraphQLFragments.NFTFragment
              + GraphQLFragments.MintNFTFragmentWithoutNFT
              + GraphQLFragments.TokenFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(new
            {
                query = NFTQuery,
                variables = new
                {
                    id = NFTId
                }
            });

            try
            {
                var response = await _client.PostAsync(request, cancellationToken);
                JObject jresponse = JObject.Parse(response.Content!);
                JToken result = jresponse["data"]!["nonFungibleToken"]!;
                return result.ToObject<NonFungibleToken>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<string?> GetNFTTransactionsResponse(int skip, int first, string nftId, CancellationToken cancellationToken = default)
        {
            var nftQuery = @"
            query nftTransactions(
                $skip: Int
                $first: Int
                $nftId: String
                $orderBy: Transaction_orderBy
                $orderDirection: OrderDirection
              ) {
                nonFungibleToken(
                  id: $nftId
                ) {
                  transactions(
                    skip: $skip
                    first: $first
                    orderBy: $orderBy
                    orderDirection: $orderDirection
                  ) {
                    id
                    __typename
                    block {
                        id
                        timestamp
                    }
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
              + GraphQLTransactionListFragments.AllFragments;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            JObject jObject = JObject.FromObject(new
            {
                query = nftQuery,
                variables = new
                {
                    skip = skip,
                    first = first,
                    nftId = nftId,
                    orderBy = "internalID",
                    orderDirection = "desc"
                }
            });
            request.AddStringBody(jObject.ToString(), ContentType.Json);
            try
            {
                var response = await _client.PostAsync(request, cancellationToken);
                return response.Content;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IList<Transaction>?> GetNFTTransactions(int skip, int first, string nftId, CancellationToken cancellationToken = default)
        {
            try
            {
                string? response = await GetNFTTransactionsResponse(skip, first, nftId, cancellationToken);
                JObject jresponse = JObject.Parse(response!);
                JToken? token = jresponse["data"]!["nonFungibleToken"]!["transactions"];
                return token!.ToObject<IList<Transaction>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new List<Transaction>();
            }
        }

        public async Task<IList<NonFungibleToken>?> GetCollectionNFTs(string tokenAddress, int skip = 0, int first = 12, string orderBy = "mintedAt", string orderDirection = "desc", CancellationToken cancellationToken = default)
        {
            var nonFungibleTokensQuery = @"
                query nonFungibleTokens($where: NonFungibleToken_filter, $skip: Int, $first: Int, $orderDirection: OrderDirection, $orderBy: NonFungibleToken_orderBy) {
                  nonFungibleTokens(where: $where, skip: $skip, first: $first, orderDirection: $orderDirection, orderBy: $orderBy) {
                    ...NFTFragment
                    __typename
                  }
                }
            "
              + GraphQLFragments.NFTFragment
              + GraphQLFragments.AccountFragment
              + GraphQLFragments.MintNFTFragmentWithoutNFT
              + GraphQLFragments.TokenFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = nonFungibleTokensQuery,
                variables = new
                {
                    where = new 
                    {
                        token_in = new List<string> { tokenAddress } //avoid invalid argument error with graph when using "token" string instead of "token_in"
                    },
                    skip = skip,
                    first = first,
                    orderBy = orderBy,
                    orderDirection = orderDirection
                }
            });
            try
            {
                var response = await _client.PostAsync(request, cancellationToken);
                JObject jresponse = JObject.Parse(response.Content!);
                JToken? token = jresponse["data"]!["nonFungibleTokens"];
                return token!.ToObject<IList<NonFungibleToken>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Swap?> GetSwapPairAndPool(Object theObject, CancellationToken cancellationToken = default)
        {
            if (theObject is Swap swap)
                if ((swap.pair != null) && (swap.pool != null) && swap.pool.balances != null)
                    return swap;

            var swapQuery = @"
            query swapQuery(
                $where: Swap_filter
              ) {
                swaps(
                  first: 1  
                  where: $where
                ) {
                  id
                  pool {
                    id
                    address
                    __typename
                    balances {
                      __typename
                      balance
                      token
                      {
                        ...TokenFragment
                      }
                    }
                  }
                  pair {
                    id
                    __typename
                    token0
                    {
                      ...TokenFragment
                    }
                    token1
                    {
                      ...TokenFragment
                    }
                  }
                }
             }"
              + GraphQLFragments.TokenFragment;
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            JObject jObject = JObject.FromObject(new
            {
                query = swapQuery,
                variables = new
                {
                    first = 1,
                    where = new
                    {
                    }
                }
            });
            JObject where = (jObject["variables"]!["where"] as JObject)!;
            try
            {
                if (theObject is Swap swap2)
                {
                    where.Add(new JProperty("id", swap2.id));
                }
                else if (theObject is Pool pool)
                {
                    where.Add(new JProperty("pool", pool.id));
                }
                else if (theObject is Pair pair)
                {
                    where.Add(new JProperty("pair", pair.id));
                }
                else throw new ArgumentException($"GetSwapPairAndPool can only be called with swap, pool or pair, not {nameof(theObject)}");

                request.AddStringBody(jObject.ToString(), ContentType.Json);
                var response = await _client.PostAsync(request, cancellationToken);
                JObject jresponse = JObject.Parse(response.Content!);
                JToken result = jresponse["data"]!["swaps"]!;
                return result.ToObject<IList<Swap>>()?.FirstOrDefault<Swap>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Remove?> GetAnyRemoveWithTokenID(string tokenID, CancellationToken cancellationToken = default)
        {
            var swapQuery = @"
            query removeQuery(
                $where: Remove_filter
              ) {
                removes(
                  first: 1  
                  where: $where
                ) {
                  id
                  pool {
                    id
                    address
                    __typename
                  }
                  token
                  {
                      ...TokenFragment
                  }
                }
             }"
              + GraphQLFragments.TokenFragment;

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            JObject jObject = JObject.FromObject(new
            {
                query = swapQuery,
                variables = new
                {
                    first = 1,
                    where = new
                    {
                        token = tokenID
                    }
                }
            });
            request.AddStringBody(jObject.ToString(), ContentType.Json);
            try
            {
                var response = await _client.PostAsync(request, cancellationToken);
                JObject jresponse = JObject.Parse(response.Content!);
                JToken result = jresponse["data"]!["removes"]!;
                return result.ToObject<IList<Remove>>()?.FirstOrDefault<Remove>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<AccountNFTSlot>> GetNftHolders(string nftId, int skip = 0, int first = 25,
            string orderBy = "balance", string orderDirection = "desc", object? slotWhere = null,
            CancellationToken cancellationToken = default)
        {
            var nftHolders = @"
            query nftHolders(
                $skip: Int
                $first: Int
                $nftId: String
                $orderBy: String
                $orderDirection: String
                $slotWhere: AccountNFTSlot_filter
            )
            {
                nonFungibleToken(
                    id: $nftId
                ) 
                {
                    slots(
                        skip: $skip
                        first: $first
                        orderBy: $orderBy
                        orderDirection: $orderDirection
                        where: $slotWhere
                    ) 
                    {
                        id
                        account {
                            id
                            address
                        }
                        balance
                    }
                }
             }";
            
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            JObject jObject = JObject.FromObject(new
            {
                query = nftHolders,
                variables = new
                {
                    first = first,
                    skip = skip,
                    nftId = nftId,
                    orderBy = orderBy,
                    orderDirection = orderDirection,
                    slotWhere = slotWhere
                }
            });
            request.AddStringBody(jObject.ToString(), ContentType.Json);
            try
            {
                var response = await _client.PostAsync(request, cancellationToken);
                JObject jresponse = JObject.Parse(response.Content!);
                JToken result = jresponse["data"]!["nonFungibleToken"]!["slots"]!;
                return result.ToObject<List<AccountNFTSlot>>()!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new List<AccountNFTSlot>();
            }
        }

        public async Task<List<Token>?> GetTokens(int skip = 0, int first = 10, string orderBy = "tradedVolume",
            string orderDirection = "desc", CancellationToken cancellationToken = default)
        {
            var tokensQuery = @"
             query tokens(
                $skip: Int
                $first: Int
                $orderBy: Token_orderBy
                $orderDirection: OrderDirection
              ) {
                tokens(
                  skip: $skip
                  first: $first
                  orderBy: $orderBy
                  orderDirection: $orderDirection
                ) {
                  id
                  name
                  symbol
                  decimals
                  address
                  tradedVolume
                }
              }
            ";

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = tokensQuery,
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
                var response = await _client.PostAsync(request, cancellationToken);
                JObject jresponse = JObject.Parse(response.Content!);
                return jresponse["data"]!["tokens"]!.ToObject<List<Token>>()!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Token?> GetToken(string tokenID)
        {
            var tokenQuery = @"
             query token(
                $tokenID: ID!
              ) {
                token(
                  id: $tokenID
                ) {
                  id
                  name
                  symbol
                  decimals
                  address
                  tradedVolume
                  tradedVolumeSwap
                  tradedVolumeOrderbook
                }
              }
            ";

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = tokenQuery,
                variables = new
                {
                    tokenID
                }
            });
            try
            {
                var response = await _client.PostAsync(request);
                JObject jresponse = JObject.Parse(response.Content!);
                JToken? jtoken = jresponse["data"]!["token"];
                return jtoken!.ToObject<Token>()!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<AccountTokenBalance>?> GetTokenHolders(string tokenId, int skip = 0, int first = 25, CancellationToken cancellationToken = default)
        {
            var tokenHoldersQuery = @"
            query tokenHolders(
                $tokenId: String
                $first: Int
                $skip: Int
            ){
                accountTokenBalances(
                    orderDirection: desc
                    orderBy: balance
                    where: {token: $tokenId}
                    first: $first
                    skip: $skip
                ) {
                    balance
                    account {
                        address
                        id
                        __typename
                    }
                }
            }
            ";

            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                query = tokenHoldersQuery,
                variables = new
                {
                    tokenId,
                    first,
                    skip
                }
            });
            try
            {
                var response = await _client.PostAsync(request, cancellationToken);
                JObject jresponse = JObject.Parse(response.Content!);
                JToken? jtoken = jresponse["data"]!["accountTokenBalances"];
                return jtoken!.ToObject<List<AccountTokenBalance>>()!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
