using Lexplorer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Lexplorer.Helpers;

namespace Lexplorer.Services;
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

    protected virtual void DoBuildLine(Transaction transaction, string? id, string? timestamp, string? type, string? from = null, string? to = null,
        string? added = null, string? addedToken = null, string? fee = null, string? feeToken = null, string? total = null, string? totalToken = null)
    //helper with default param values for more readable code
    {
        BuildLine(id, timestamp, type, from, to, added, addedToken, fee, feeToken, total, totalToken);
    }

    private void WriteOrderBookTrade(OrderBookTrade orderBookTrade, string accountIdPerspective, CSVWriteLine writeLine)
    {
        bool areWeAccountA = orderBookTrade.accountA?.id == accountIdPerspective;
        if (areWeAccountA)
        {
            DoBuildLine(orderBookTrade, orderBookTrade.id, orderBookTrade.verifiedAt, orderBookTrade.typeName,
                from: orderBookTrade.accountA?.address, to: orderBookTrade.accountB?.address,
                added: GetExportAmount(orderBookTrade.fillBA - orderBookTrade.feeA, orderBookTrade.tokenB), //Amount of token B bought by Account A
                addedToken: orderBookTrade.tokenB?.symbol,
                fee: GetExportAmount(orderBookTrade.feeA, orderBookTrade.tokenB), //Fee paid by Account A with Token B
                feeToken: orderBookTrade.tokenB?.symbol,
                total: GetExportAmount(orderBookTrade.fillSA, orderBookTrade.tokenA), //Amount of token A sold by Account A
                totalToken: orderBookTrade.tokenA?.symbol
                );
        }
        else
        {
            DoBuildLine(orderBookTrade, orderBookTrade.id, orderBookTrade.verifiedAt, orderBookTrade.typeName,
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
            DoBuildLine(swap, swap.id, swap.verifiedAt, swap.typeName,
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
        DoBuildLine(transfer, transfer.id, transfer.verifiedAt, transfer.typeName,
            from: transfer.fromAccount?.address,
            to: transfer.toAccount?.address,
            added: (transfer.toAccount?.id == accountIdPerspective) ? GetExportAmount(transfer.amount, transfer.token) : null,
            addedToken: (transfer.toAccount?.id == accountIdPerspective) ? transfer.token?.symbol : null,
            fee: GetExportAmount(transfer.fee, transfer.feeToken),
            feeToken: transfer.feeToken?.symbol,
            total: (transfer.fromAccount?.id == accountIdPerspective) ? GetExportAmount(transfer.amount, transfer.token) : null,
            totalToken: (transfer.fromAccount?.id == accountIdPerspective) ? transfer.token?.symbol : null);
    }
    private void WriteDeposit(Deposit deposit, string accountIdPerspective, CSVWriteLine writeLine)
    {
        DoBuildLine(deposit, deposit.id, deposit.verifiedAt, deposit.typeName,
            to: deposit.toAccount?.address,
            added: GetExportAmount(deposit.amount, deposit.token),
            addedToken: deposit.token?.symbol);
    }
    private void WriteWithdrawal(Withdrawal withdrawal, string accountIdPerspective, CSVWriteLine writeLine)
    {
        DoBuildLine(withdrawal, withdrawal.id, withdrawal.verifiedAt, withdrawal.typeName,
            from: withdrawal.fromAccount?.address,
            total: GetExportAmount(withdrawal.amount, withdrawal.token),
            totalToken: withdrawal.token?.symbol);
    }
    private void WriteAccountUpdate(AccountUpdate accountUpdate, string accountIdPerspective, CSVWriteLine writeLine)
    {
        DoBuildLine(accountUpdate, accountUpdate.id, accountUpdate.verifiedAt, accountUpdate.typeName,
            from: accountUpdate.user?.address,
            fee: GetExportAmount(accountUpdate.fee, accountUpdate.feeToken),
            feeToken: accountUpdate.feeToken?.symbol);
    }
    private void WriteTransferNFT(TransferNFT transferNFT, string accountIdPerspective, CSVWriteLine writeLine)
    {
        DoBuildLine(transferNFT, transferNFT.id, transferNFT.verifiedAt, transferNFT.typeName,
            from: transferNFT.fromAccount?.address,
            to: transferNFT.toAccount?.address,
            added: (transferNFT.toAccount?.id == accountIdPerspective) ? transferNFT.amount.ToString() : null,
            addedToken: (transferNFT.toAccount?.id == accountIdPerspective) ? "NFT" : null,
            fee: GetExportAmount(transferNFT.fee, transferNFT.feeToken),
            feeToken: transferNFT.feeToken?.symbol,
            total: (transferNFT.fromAccount?.id == accountIdPerspective) ? transferNFT.amount.ToString() : null,
            totalToken: (transferNFT.fromAccount?.id == accountIdPerspective) ? "NFT" : null);
    }
    private void WriteMintNFT(MintNFT mintNFT, string accountIdPerspective, CSVWriteLine writeLine)
    {
        DoBuildLine(mintNFT, mintNFT.id, mintNFT.verifiedAt, mintNFT.typeName,
            from: mintNFT.minter?.address,
            to: mintNFT.receiver?.address,
            added: (mintNFT.receiver?.id == accountIdPerspective) ? mintNFT.amount.ToString() : null,
            addedToken: (mintNFT.receiver?.id == accountIdPerspective) ? "NFT" : null,
            fee: (mintNFT.receiver?.id == accountIdPerspective) ? GetExportAmount(mintNFT.fee, mintNFT.feeToken) : null,
            feeToken: (mintNFT.receiver?.id == accountIdPerspective) ? mintNFT.feeToken?.symbol : null,
            total: (mintNFT.minter?.id == accountIdPerspective) && (mintNFT.minter?.id != mintNFT.receiver?.id) ? mintNFT.amount.ToString() : null,
            totalToken: (mintNFT.minter?.id == accountIdPerspective) && (mintNFT.minter?.id != mintNFT.receiver?.id) ? "NFT" : null);
    }
    private void WriteWithdrawalNFT(WithdrawalNFT withdrawalNFT, string accountIdPerspective, CSVWriteLine writeLine)
    {
        DoBuildLine(withdrawalNFT, withdrawalNFT.id, withdrawalNFT.verifiedAt, withdrawalNFT.typeName,
            from: withdrawalNFT.fromAccount?.address,
            to: null,
            added: null,
            addedToken: null,
            fee: (withdrawalNFT.fromAccount?.id == accountIdPerspective) ? GetExportAmount(withdrawalNFT.fee, withdrawalNFT.feeToken) : null,
            feeToken: (withdrawalNFT.fromAccount?.id == accountIdPerspective) ? withdrawalNFT.feeToken?.symbol : null,
            total: (withdrawalNFT.fromAccount?.id == accountIdPerspective) ? withdrawalNFT.amount.ToString() : null,
            totalToken: (withdrawalNFT.fromAccount?.id == accountIdPerspective) ? "NFT" : null);
    }
    private static readonly List<string> transactionTypesToIgnore = new()
    {
        { "SignatureVerification" },
        { "AmmUpdate" }
    };
    public void WriteTransaction(Transaction transaction, string accountIdPerspective, CSVWriteLine writeLine)
    {
        sb.Clear();
        if (transaction is OrderBookTrade orderBookTrade)
            WriteOrderBookTrade(orderBookTrade, accountIdPerspective, writeLine);
        else if (transaction is Transfer transfer)
            WriteTransfer(transfer, accountIdPerspective, writeLine);
        else if (transaction is Deposit deposit)
            WriteDeposit(deposit, accountIdPerspective, writeLine);
        else if (transaction is Withdrawal withdrawal)
            WriteWithdrawal(withdrawal, accountIdPerspective, writeLine);
        else if (transaction is Swap swap)
            WriteSwap(swap, accountIdPerspective, writeLine);
        else if (transaction is TransferNFT transferNFT)
            WriteTransferNFT(transferNFT, accountIdPerspective, writeLine);
        else if (transaction is MintNFT mintNFT)
            WriteMintNFT(mintNFT, accountIdPerspective, writeLine);
        else if (transaction is WithdrawalNFT withdrawalNFT)
            WriteWithdrawalNFT(withdrawalNFT, accountIdPerspective, writeLine);
        else if (transaction is AccountUpdate accountUpdate)
            WriteAccountUpdate(accountUpdate, accountIdPerspective, writeLine);
        else if (transactionTypesToIgnore.Contains(transaction.typeName!))
#pragma warning disable CS0642 // Possible mistaken empty statement
            ; //nothing to export
#pragma warning restore CS0642 // Possible mistaken empty statement
        else
            sb.AppendJoin(Convert.ToChar(9), transaction.id, transaction.verifiedAt, transaction.typeName);

        writeLine(sb.ToString());
    }
}
