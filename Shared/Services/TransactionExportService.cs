using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Lexplorer.Models;

namespace Lexplorer.Services
{
    public delegate void CSVWriteLine(string line);
    public interface ICSVFormatService
    {
        public void WriteHeader(CSVWriteLine writeLine);
        public void WriteTransaction(Transaction transaction, string accountIdPerspective,  CSVWriteLine writeLine);
    }

    public class TransactionExportService
    {
        private readonly LoopringGraphQLService _graphqlService;

        public TransactionExportService(LoopringGraphQLService graphQLService)
        {
            _graphqlService = graphQLService;
        }

        private ICSVFormatService getFormatService(string CSVFormat)
        {
            //todo: make dynamic, register various formats etc.
            return new TranscationExportDefaultCSVFormat();
        }
        public async Task<Stream> GenerateCSV(string CSVFormat, string accountId, DateTime? startDate, DateTime? endDate)
        {
            var format = getFormatService(CSVFormat);

            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                CSVWriteLine writeLine = (string line) => writer.WriteLine(line);
                format.WriteHeader(writeLine);
                var processed = 0;
                while (true)
                {
                    const int chunkSize = 10;
                    IList<Transaction>? transactions = await _graphqlService.GetAccountTransactions(processed, chunkSize, accountId, startDate, endDate)!;
                    if ((transactions == null) || (transactions.Count == 0)) break;
                    foreach (var transaction in transactions)
                    {
                        format.WriteTransaction(transaction, accountId, writeLine);
                    }
                    if (transactions.Count < chunkSize) break;
                    processed += chunkSize;
                }
            }
            stream.Position = 0;
            return stream;
        }
    }
}
