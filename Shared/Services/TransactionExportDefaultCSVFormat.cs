using Lexplorer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Lexplorer.Helpers;

namespace Lexplorer.Services
{
    public class TransactionExportDefaultCSVFormat : ICSVFormatService
    {
        protected readonly StringBuilder sb = new StringBuilder();

        public virtual void SuggestFileName(ref string fileName, string accountId, DateTime startDate, DateTime endDate)
        {
            //leave fileName unchanged
        }

        public virtual void BuildLine(params string?[] columns)
        {
            sb.AppendJoin(Convert.ToChar(9), columns);
        }

        public virtual void WriteHeader(CSVWriteLine writeLine)
        {
            sb.Clear();
            BuildLine("Tx-ID", "Timestamp", "Type", "From", "To", "Added", "Added Token", "Fee", "Fee Token", "Total", "Total Token");
            writeLine(sb.ToString());
        }

        protected string? GetExportAmount(double amount, Token? token)
        {
            return TokenAmountConverter.ToDecimal(amount, token?.decimals).ToString("");
        }

        protected virtual void DoBuildLine(string? id, string? timestamp, string? type, string? from = null, string? to = null, 
            string? added = null, string? addedToken = null, string? fee = null, string? feeToken = null, string? total = null, string? totalToken = null)
        //helper with default param values for more readable code
        {
            BuildLine(id, timestamp, type, from, to, added, addedToken, fee, feeToken, total, totalToken);
        }

        private void WriteOrderBookTrade(OrderBookTrade orderBookTrade, string accountIdPerspective, CSVWriteLine writeLine)
        {
            bool areWeAccountA = orderBookTrade.accountA?.id == accountIdPerspective;
            bool buyOrSellForUs = areWeAccountA ? orderBookTrade.fillAmountBorSA : orderBookTrade.fillAmountBorSB;
            if (areWeAccountA)
            {
                //for now we're only checking fillAmountBorSA, because we're A, but worst case would be all 4 combinations with fillAmoutBorSB!
                if (orderBookTrade.fillAmountBorSA)
                {
                    throw new InvalidOperationException("OrderBookTrade with fillAmountBorSA = true has no implemented export for now");
                }
                else
                {
                    DoBuildLine(orderBookTrade.id, orderBookTrade.verifiedAt, orderBookTrade.typeName,
                        from: orderBookTrade.accountA?.address, to: orderBookTrade.accountB?.address,
                        added: GetExportAmount(orderBookTrade.fillBA - orderBookTrade.feeA, orderBookTrade.tokenB), //Amount of token B bought by Account A
                        addedToken: orderBookTrade.tokenB?.symbol,
                        fee: GetExportAmount(orderBookTrade.feeA, orderBookTrade.tokenB), //Fee paid by Account A with Token B
                        feeToken: orderBookTrade.tokenB?.symbol,
                        total: GetExportAmount(orderBookTrade.fillSA, orderBookTrade.tokenA), //Amount of token A sold by Account A
                        totalToken: orderBookTrade.tokenA?.symbol
                        );
                }
            }
            else
            {
                //for now we're only checking fillAmountBorSB, because we're B, but worst case would be all 4 combinations with fillAmoutBorSA!
                if (orderBookTrade.fillAmountBorSB)
                {
                    throw new InvalidOperationException("OrderBookTrade with fillAmountBorSB = true has no implemented export for now");
                }
                else
                {
                    DoBuildLine(orderBookTrade.id, orderBookTrade.verifiedAt, orderBookTrade.typeName,
                        from: orderBookTrade.accountA?.address, to: orderBookTrade.accountB?.address,
                        added: GetExportAmount(orderBookTrade.fillBB, orderBookTrade.tokenA), //Amount of token A bought by Account B
                        addedToken: orderBookTrade.tokenA?.symbol,
                        fee: GetExportAmount(orderBookTrade.feeA, orderBookTrade.tokenB), //doc wrong: Fee paid by Account A with Token B
                        feeToken: orderBookTrade.tokenB?.symbol,
                        total: GetExportAmount(orderBookTrade.fillSB, orderBookTrade.tokenB), //Amount of token B sold by Account B
                        totalToken: orderBookTrade.tokenB?.symbol
                        );
                }
            }
        }
        private void WriteSwap(Swap swap, string accountIdPerspective, CSVWriteLine writeLine)
        {
            if (swap.fillAmountBorSA)
            {
                //also did not find *any* Swap with fillAmountBorSA = true!
                throw new InvalidOperationException("Swap with fillAmountBorSA = true has no implemented export for now");
            }
            else
            {
                //we are account A and bought token B, selling token A, paying feeA in tokenB
                DoBuildLine(swap.id, swap.verifiedAt, swap.typeName,
                    from: swap.account?.address,
                    to: swap.pool?.address,
                    added: GetExportAmount(swap.fillBA - swap.feeA, swap.tokenB), //Amount of token B bought by Account A minus feeA we paid
                    addedToken: swap.tokenB?.symbol,
                    fee: GetExportAmount(swap.feeA, swap.tokenB),  //Fee paid by Account A with Token B
                    feeToken: swap.tokenB?.symbol,
                    total: GetExportAmount(swap.fillSA, swap.tokenA), //Amount of token A sold by Account A
                    totalToken: swap.tokenA?.symbol
                    );
            }
        }
        private void WriteTransfer(Transfer transfer, string accountIdPerspective, CSVWriteLine writeLine)
        {
            DoBuildLine(transfer.id, transfer.verifiedAt, transfer.typeName,
                from: transfer.fromAccount?.address,
                to: transfer.toAccount?.address,
                added: (transfer.toAccount?.id == accountIdPerspective) ? GetExportAmount(transfer.amount, transfer.token) : null,
                addedToken: (transfer.toAccount?.id == accountIdPerspective) ? transfer.token?.symbol : null,
                fee: GetExportAmount(transfer.fee, transfer.feeToken),
                feeToken: transfer.feeToken?.symbol,
                total: (transfer.fromAccount?.id == accountIdPerspective) ? GetExportAmount(transfer.amount, transfer.token) : null,
                totalToken: (transfer.fromAccount?.id  == accountIdPerspective) ? transfer.token?.symbol : null);
        }
        private void WriteDeposit(Deposit deposit, string accountIdPerspective, CSVWriteLine writeLine)
        {
            DoBuildLine(deposit.id, deposit.verifiedAt, deposit.typeName,
                to: deposit.toAccount?.address,
                added: GetExportAmount(deposit.amount, deposit.token),
                addedToken: deposit.token?.symbol);
        }
        private void WriteWithdrawal(Withdrawal withdrawal, string accountIdPerspective, CSVWriteLine writeLine)
        {
            DoBuildLine(withdrawal.id, withdrawal.verifiedAt, withdrawal.typeName,
                from: withdrawal.fromAccount?.address,
                total: GetExportAmount(withdrawal.amount, withdrawal.token),
                totalToken: withdrawal.token?.symbol);
        }
        private void WriteAccountUpdate(AccountUpdate accountUpdate, string accountIdPerspective, CSVWriteLine writeLine)
        {
            DoBuildLine(accountUpdate.id, accountUpdate.verifiedAt, accountUpdate.typeName, 
                from: accountUpdate.user?.address, 
                fee: GetExportAmount(accountUpdate.fee, accountUpdate.feeToken),
                feeToken: accountUpdate.feeToken?.symbol);
        }
        private void WriteTransferNFT(TransferNFT transferNFT, string accountIdPerspective, CSVWriteLine writeLine)
        {
            DoBuildLine(transferNFT.id, transferNFT.verifiedAt, transferNFT.typeName,
                from: transferNFT.fromAccount?.address,
                to: transferNFT.toAccount?.address,
                added: (transferNFT.toAccount?.id == accountIdPerspective) ? transferNFT.amount.ToString() : null,
                addedToken: (transferNFT.toAccount?.id == accountIdPerspective) ? "NFT" : null,
                fee: GetExportAmount(transferNFT.fee, transferNFT.feeToken),
                feeToken: transferNFT.feeToken?.symbol,
                total: (transferNFT.fromAccount?.id == accountIdPerspective) ? transferNFT.amount.ToString() : null,
                totalToken: (transferNFT.fromAccount?.id == accountIdPerspective) ? "NFT" : null);
        }
        public void WriteTransaction(Transaction transaction, string accountIdPerspective, CSVWriteLine writeLine)
        {
            sb.Clear();
            if (transaction is OrderBookTrade)
                WriteOrderBookTrade((OrderBookTrade)transaction, accountIdPerspective, writeLine);
            else if (transaction is Transfer)
                WriteTransfer((Transfer)transaction, accountIdPerspective, writeLine);
            else if (transaction is Deposit)
                WriteDeposit((Deposit)transaction, accountIdPerspective, writeLine);
            else if (transaction is Withdrawal)
                WriteWithdrawal((Withdrawal)transaction, accountIdPerspective, writeLine);
            else if (transaction is Swap)
                WriteSwap((Swap)transaction, accountIdPerspective, writeLine);
            else if (transaction is TransferNFT)
                WriteTransferNFT((TransferNFT)transaction, accountIdPerspective, writeLine);
            else if (transaction is AccountUpdate)
                WriteAccountUpdate((AccountUpdate)transaction, accountIdPerspective, writeLine);
            else if (transaction.typeName == "SignatureVerification")
#pragma warning disable CS0642 // Possible mistaken empty statement
                ; //nothing to export
#pragma warning restore CS0642 // Possible mistaken empty statement
            else
                sb.AppendJoin(Convert.ToChar(9), transaction.id, transaction.verifiedAt, transaction.typeName);

            writeLine(sb.ToString());
        }
    }
}
