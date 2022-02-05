using GraphQL.Client.Http;

namespace Lexplorer.Models
{
    public static class GraphQLConstants
    {

        public static GraphQL.GraphQLRequest FetchBlocksGraphlQLQuery = new GraphQLHttpRequest
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
            "
            + BlockFragment,
            Variables = new
            {
                skip = 0,
                first = 10,
                orderBy = "internalID",
                orderDirection = "desc"
            }
        };

        public static string BlockFragment = 
            @"fragment BlockFragment on Block {
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
              }"
            + AccountFragment;

        public static string AccountFragment =
            @"  fragment AccountFragment on Account {
            id
            address
            }";
    }
}

