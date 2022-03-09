using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexplorer.Services
{
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
            fileName = $"Cointracking_{accountId}_{startDate.ToString("yyyy-MM-dd")}_{endDate.ToString("yyyy-MM-dd")}.csv";
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

        protected override void DoBuildLine(string? id, string? timestamp, string? type, string? from = null, string? to = null,
            string? added = null, string? addedToken = null, string? fee = null, string? feeToken = null, string? total = null, string? totalToken = null)
        {
            BuildLine(type, added, addedToken, total, totalToken, fee, feeToken, "Loopring DEX", "", "", timestamp, id, "", "");
        }

    }
}
