using Lexplorer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexplorer.Services
{
    public class TranscationExportDefaultCSVFormat : ICSVFormatService
    {
        private readonly StringBuilder sb = new StringBuilder();
        public void WriteHeader(CSVWriteLine writeLine)
        {
            sb.Clear();
            sb.AppendJoin(Convert.ToChar(9), "Tx-ID", "Timestamp", "Type");
            writeLine(sb.ToString());
        }
        public void WriteTransaction(Transaction transaction, string accountIdPerspective, CSVWriteLine writeLine)
        {
            sb.Clear();
            sb.AppendJoin(Convert.ToChar(9), transaction.id, transaction.verifiedAt, transaction.typeName);
            writeLine(sb.ToString());
        }
    }
}
