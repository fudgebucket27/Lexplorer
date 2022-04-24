using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexplorer.Models;

namespace Lexplorer.Services;
public class TransactionExportCointracking : TransactionExportDefaultCSVFormat
{
    private string? quotedString(string? text)
    {
        if (text == null) return null;
        if (text.Contains(",", StringComparison.InvariantCulture) || text.Contains($"\"") || text.StartsWith(" ") || text.EndsWith(" "))
        {
            string quoted = $"\"" + text.Replace($"\"", "\"\"") + $"\"";
            return quoted;
        }
        else
            return text;
    }

    public override void SuggestFileName(ref string fileName, string accountId, DateTime startDate, DateTime endDate)
    {
        fileName = $"Cointracking_{accountId}_{startDate:yyyy-MM-dd}_{endDate:yyyy-MM-dd}.csv";
    }

    public override void BuildLine(params string?[] columns)
    {
        List<string?> modifiedColumns = columns.Select(item => quotedString(item)).ToList();
        sb.AppendJoin(",", modifiedColumns);
    }

    public override void WriteHeader(CSVWriteLine writeLine)
    {
        sb.Clear();
        BuildLine("Type", "Buy Amount", "Buy Currency", "Sell Amount", "Sell Currency", "Fee", "Fee Currency", "Exchange", "Trade-Group", "Comment", "Date",
            "Tx-ID", "Buy Value in Account Currency", "Sell Value in Account Currency");
        writeLine(sb.ToString());
    }

    private static readonly Dictionary<string, string> typeDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Swap", "Trade" },
            { "OrderBookTrade", "Trade" },
            { "MintNFT", "Other Fee" },
            { "WithdrawalNFT", "Withdrawal" }
        };

    protected override void DoBuildLine(Transaction transaction, string? id, string? timestamp, string? type, string? from = null, string? to = null,
        string? added = null, string? addedToken = null, string? fee = null, string? feeToken = null, string? total = null, string? totalToken = null)
    {
        //type names taken from https://cointracking.info/import/import_xls/?language=en
        //some are easy to translate, for some it depends whether we received or send something
        if (typeDict.TryGetValue(type!, out string? newType))
            type = newType!;
        else
        {
            if (transaction is TransferNFT)
            {
                type = addedToken != null ? "Gift" : "Donation";
            }
            else if (transaction is Transfer)
            {
                type = addedToken != null ? "Deposit" : "Withdrawal";
            }
        }
        BuildLine(type, added, addedToken, total, totalToken, fee, feeToken, "Loopring L2 DEX", "", "", timestamp, id, "", "");
    }

}
