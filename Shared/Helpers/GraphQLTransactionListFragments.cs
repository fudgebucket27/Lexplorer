using System;

namespace Lexplorer.Helpers
{
	public static class GraphQLTransactionListFragments
	{
        //this class collects more compact and less nested fragments used when
        //querying lists of transactions

        public static string Account = @"
            fragment AccountFragment on Account {
                id
                address
                __typename
            }";

        public static string Token = @"
            fragment TokenFragment on Token {
                id
                name
                symbol
                decimals
              }";

        public static string Swap = @"
        fragment SwapFragment on Swap {
            id
            account {
              ...AccountFragment
            }
            pool {
              ...AccountFragment
            }
            tokenA {
              ...TokenFragment
            }
            tokenB {
              ...TokenFragment
            }
            fillSA
            fillSB
            fillBA
            fillBB
            feeA
            feeB
            __typename
          }";

        public static string Add = @"
        fragment AddFragment on Add {
            id
            account {
              ...AccountFragment
            }
            pool {
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

        public static string Remove = @"
         fragment RemoveFragment on Remove {
            id
            account {
              ...AccountFragment
            }
            pool {
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

        public static string OrderBookTrade = @"
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

        public static string Deposit = @"
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

        public static string Withdrawal = @"
          fragment WithdrawalFragment on Withdrawal {
            id
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

        public static string Transfer = @"
         fragment TransferFragment on Transfer {
            id
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

        public static string AccountUpdate = @"
          fragment AccountUpdateFragment on AccountUpdate {
            id
            user {
              ...AccountFragment
            }
            feeToken {
              ...TokenFragment
            }
            fee
            __typename
          }";

        public static string AmmUpdate = @"
          fragment AmmUpdateFragment on AmmUpdate {
            id
            pool {
              ...AccountFragment
            }
            balance
            __typename
          }";

        public static string SignatureVerification = @"
          fragment SignatureVerificationFragment on SignatureVerification {
            id
            account {
              ...AccountFragment
            }
            __typename
          }";

        public static string TradeNFT = @"
          fragment TradeNFTFragment on TradeNFT {
            id
            accountSeller {
              ...AccountFragment
            }
            accountBuyer {
              ...AccountFragment
            }
            token {
              ...TokenFragment
            }
            realizedNFTPrice
            feeBuyer
            feeSeller
            fillSA
            fillBA
            fillSB
            fillBB
            tokenIDAS
            __typename
          }";

        public static string SwapNFT = @"
         fragment SwapNFTFragment on SwapNFT {
            id
            accountA {
              ...AccountFragment
            }
            accountB {
              ...AccountFragment
            }
            __typename
          }";

        public static string WithdrawalNFT = @"
          fragment WithdrawalNFTFragment on WithdrawalNFT {
            id
            fromAccount {
              ...AccountFragment
            }
            fee
            feeToken {
              ...TokenFragment
            }
            amount
            __typename
          }";

        public static string TransferNFT = @"
          fragment TransferNFTFragment on TransferNFT {
            id
            fromAccount {
              ...AccountFragment
            }
            toAccount {
              ...AccountFragment
            }
            feeToken {
              ...TokenFragment
            }
            fee
            amount
            __typename
          }";

        public static string MintNFT = @"
          fragment MintNFTFragment on MintNFT {
            id
            minter {
              ...AccountFragment
            }
            receiver {
              ...AccountFragment
            }
            fee
            feeToken {
              ...TokenFragment
            }
            amount
            __typename
          }";

        public static string DataNFT = @"
          fragment DataNFTFragment on DataNFT {
            id
            accountID
            __typename
          }";

        public static string AllFragments =
            Account
              + Token
              + Add
              + Remove
              + Swap
              + OrderBookTrade
              + Deposit
              + Withdrawal
              + Transfer
              + AccountUpdate
              + AmmUpdate
              + SignatureVerification
              + TradeNFT
              + SwapNFT
              + WithdrawalNFT
              + TransferNFT
              + MintNFT
              + DataNFT;

    }
}

