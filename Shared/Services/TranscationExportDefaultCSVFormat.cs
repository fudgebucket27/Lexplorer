using Lexplorer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Lexplorer.Helpers;

namespace Lexplorer.Services
{
    public class TranscationExportDefaultCSVFormat : ICSVFormatService
    {
        private readonly StringBuilder sb = new StringBuilder();
        public void WriteHeader(CSVWriteLine writeLine)
        {
            sb.Clear();
            sb.AppendJoin(Convert.ToChar(9), "Tx-ID", "Timestamp", "Type", "From", "To", "Added", "Added Token", "Fee", "Fee Token", "Total", "Total Token");
            writeLine(sb.ToString());
        }

        private string? GetExportAmount(double amount, Token? token)
        {
            return TokenAmountConverter.ToString(amount, token?.decimals ?? 1, 1, "");
        }

        //helper with default param values for more readable code
        private void DoBuildLine(string? id, string? timestamp, string? type, string? from = null, string? to = null, 
            string? added = null, string? addedToken = null, string? fee = null, string? feeToken = null, string? total = null, string? totalToken = null)
        {
            sb.AppendJoin(Convert.ToChar(9), id, timestamp, type, from, to, added, addedToken, fee, feeToken, total, totalToken);
        }
        private void WriteOrderBookTrade(OrderBookTrade orderBookTrade, string accountIdPerspective, CSVWriteLine writeLine)
        {
            bool areWeAccountA = orderBookTrade.accountA?.id == accountIdPerspective;
            bool buyOrSellForUs = areWeAccountA ? orderBookTrade.fillAmountBorSA : orderBookTrade.fillAmountBorSB;
            if (areWeAccountA)
            {
                if (orderBookTrade.fillAmountBorSA)
                {
                    DoBuildLine(orderBookTrade.id, orderBookTrade.verifiedAt, orderBookTrade.typeName);
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
                if (orderBookTrade.fillAmountBorSA)
                {
                    DoBuildLine(orderBookTrade.id, orderBookTrade.verifiedAt, orderBookTrade.typeName);
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
                //a buy for us, 
                DoBuildLine(swap.id, swap.verifiedAt, swap.typeName,
                    from: swap.account?.address,
                    to: swap.pool?.address


                    );
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
                added: TokenAmountConverter.ToString(transfer.amount, transfer.token?.decimals ?? 1, 1, ""),
                addedToken: transfer.token?.symbol,
                fee: TokenAmountConverter.ToString(transfer.fee, transfer.feeToken?.decimals ?? 1, 1, ""),
                feeToken: transfer.feeToken?.symbol);
        }
        private void WriteAccountUpdate(AccountUpdate accountUpdate, string accountIdPerspective, CSVWriteLine writeLine)
        {
            DoBuildLine(accountUpdate.id, accountUpdate.verifiedAt, accountUpdate.typeName, 
                from: accountUpdate.user?.address, 
                fee: TokenAmountConverter.ToString(accountUpdate.fee, accountUpdate.feeToken?.decimals ?? 1, 1, ""),
                feeToken: accountUpdate.feeToken?.symbol);
        }
        public void WriteTransaction(Transaction transaction, string accountIdPerspective, CSVWriteLine writeLine)
        {
            sb.Clear();
            if (transaction is OrderBookTrade)
                WriteOrderBookTrade((OrderBookTrade)transaction, accountIdPerspective, writeLine);
            else if (transaction is Transfer)
                WriteTransfer((Transfer)transaction, accountIdPerspective, writeLine);
            else if (transaction is Swap)
                WriteSwap((Swap)transaction, accountIdPerspective, writeLine);
            else if (transaction is AccountUpdate)
                WriteAccountUpdate((AccountUpdate)transaction, accountIdPerspective, writeLine);
            else
                sb.AppendJoin(Convert.ToChar(9), transaction.id, transaction.verifiedAt, transaction.typeName);

            writeLine(sb.ToString());
        }
    }
}
