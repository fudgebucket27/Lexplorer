namespace Lexplorer.Helpers
{
    public static class GraphQLFragments
    {
        public static string BlockFragment = @"  
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
              }";

        public static string AccountFragment = @"
            fragment AccountFragment on Account {
                id
                address
                __typename
            }";

        public static string TokenFragment = @"
            fragment TokenFragment on Token {
                id
                name
                symbol
                decimals
                address
              }";

        public static string PoolFragment = @"
            fragment PoolFragment on Pool {
                id
                address
                balances {
                  id
                  balance
                  token {
                    ...TokenFragment
                  }
                }
              }";

        public static string NFTFragment = @"
          fragment NFTFragment on NonFungibleToken {
            id
            minter {
              ...AccountFragment
            }
            nftID
            nftType
            token
          }
        ";

        public static string AddFragment = @"
        fragment AddFragment on Add {
            id
            account {
              ...AccountFragment
            }
            pool {
              ...PoolFragment
            }
            token {
              ...TokenFragment
            }
            feeToken {
              ...TokenFragment
            }
            amount
            fee
            __typename
          }";

        public static string RemoveFragment = @"
         fragment RemoveFragment on Remove {
            id
            account {
              ...AccountFragment
            }
            pool {
              ...PoolFragment
            }
            token {
              ...TokenFragment
            }
            feeToken {
              ...TokenFragment
            }
            amount
            fee
            __typename
          }";

        public static string SwapFragment = @"
        fragment SwapFragment on Swap {
            id
            account {
              ...AccountFragment
            }
            pool {
              ...PoolFragment
            }
            tokenA {
              ...TokenFragment
            }
            tokenB {
              ...TokenFragment
            }
            pair {
              id
              token0 {
                symbol
              }
              token1 {
                symbol
              }
            }
            tokenAPrice
            tokenBPrice
            fillSA
            fillSB
            fillBA
            protocolFeeA
            protocolFeeB
            feeA
            feeB
            __typename
          }";

        public static string OrderBookTradeFragment = @"
          fragment OrderbookTradeFragment on OrderbookTrade {
            id
            accountA {
              ...AccountFragment
            }
            accountB {
              ...AccountFragment
            }
            tokenA {
              ...TokenFragment
            }
            tokenB {
              ...TokenFragment
            }
            pair {
              id
              token0 {
                symbol
              }
              token1 {
                symbol
              }
            }
            tokenAPrice
            tokenBPrice
            fillSA
            fillSB
            fillBA
            fillBB
            fillAmountBorSA
            fillAmountBorSB
            feeA
            feeB
            __typename
          }";

        public static string DepositFragment = @"
          fragment DepositFragment on Deposit {
            id
            toAccount {
              ...AccountFragment
            }
            token {
              ...TokenFragment
            }
            amount
            __typename
          }";

        public static string WithdrawalFragment = @"
          fragment WithdrawalFragment on Withdrawal {
            fromAccount {
              ...AccountFragment
            }
            token {
              ...TokenFragment
            }
            feeToken {
              ...TokenFragment
            }
            amount
            fee
            __typename
          }";

        public static string TransferFragment = @"
         fragment TransferFragment on Transfer {
            fromAccount {
              ...AccountFragment
            }
            toAccount {
              ...AccountFragment
            }
            token {
              ...TokenFragment
            }
            feeToken {
              ...TokenFragment
            }
            amount
            fee
            __typename
          }";

        public static string AccountUpdateFragment = @"
          fragment AccountUpdateFragment on AccountUpdate {
            user {
              id
              address
              publicKey
            }
            feeToken {
              ...TokenFragment
            }
            fee
            nonce
            __typename
          }";

        public static string AmmUpdateFragment = @"
          fragment AmmUpdateFragment on AmmUpdate {
            pool {
              ...PoolFragment
            }
            tokenID
            feeBips
            tokenWeight
            nonce
            balance
            tokenBalances {
              id
              balance
              token {
                ...TokenFragment
              }
            }
            __typename
          }";

        public static string SignatureVerificationFragment = @"
          fragment SignatureVerificationFragment on SignatureVerification {
            account {
              ...AccountFragment
            }
            verificationData
            __typename
          }";

        public static string TradeNFTFragment = @"
          fragment TradeNFTFragment on TradeNFT {
            accountSeller {
              ...AccountFragment
            }
            accountBuyer {
              ...AccountFragment
            }
            token {
              ...TokenFragment
            }
            nfts {
              ...NFTFragment
            }
            realizedNFTPrice
            feeBuyer
            protocolFeeBuyer
            __typename
          }";

        public static string SwapNFTFragment = @"
         fragment SwapNFTFragment on SwapNFT {
            accountA {
              ...AccountFragment
            }
            accountB {
              ...AccountFragment
            }
            nfts {
              ...NFTFragment
            }
            __typename
          }";

        public static string WithdrawalNFTFragment = @"
          fragment WithdrawalNFTFragment on WithdrawalNFT {
            fromAccount {
              ...AccountFragment
            }
            fee
            feeToken {
              ...TokenFragment
            }
            nfts {
              ...NFTFragment
            }
            __typename
          }";

        public static string TransferNFTFragment = @"
          fragment TransferNFTFragment on TransferNFT {
            fromAccount {
              ...AccountFragment
            }
            toAccount {
              ...AccountFragment
            }
            feeToken {
              ...TokenFragment
            }
            nfts {
              ...NFTFragment
            }
            fee
            __typename
          }";

        public static string MintNFTFragment = @"
          fragment MintNFTFragment on MintNFT {
            minter {
              ...AccountFragment
            }
            receiver {
              ...AccountFragment
            }
            receiverSlot {
              id
            }
            nft {
              id
            }
            fee
            feeToken {
              ...TokenFragment
            }
            amount
            __typename
          }";

        public static string DataNFTFragment = @"
        fragment DataNFTFragment on DataNFT {
            __typename
          }";
    }
}
