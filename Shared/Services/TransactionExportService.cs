﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Lexplorer.Models;
using System.Linq;

namespace Lexplorer.Services
{
    public delegate void CSVWriteLine(string line);
    public interface ICSVFormatService
    {
        public void SuggestFileName(ref string fileName, string accountId, DateTime startDate, DateTime endDate);
        public void WriteHeader(CSVWriteLine writeLine);
        public void WriteTransaction(Transaction transaction, string accountIdPerspective,  CSVWriteLine writeLine);
    }

    public class TransactionExportService
    {
        private static readonly Dictionary<string, ICSVFormatService> registeredExportServices;

        static TransactionExportService()
        {
            registeredExportServices = new Dictionary<string, ICSVFormatService>();
        }

        public static void RegisterExportService(string CSVFormatName, ICSVFormatService service)
        {
            registeredExportServices.Add(CSVFormatName, service);
        }

        private readonly LoopringGraphQLService _graphqlService;

        public TransactionExportService(LoopringGraphQLService graphQLService)
        {
            _graphqlService = graphQLService;
        }

        public ICSVFormatService getFormatService(string CSVFormat)
        {
            return registeredExportServices[CSVFormat];
        }

        public List<string> ExportFormats()
        {
            return registeredExportServices.Keys.ToList<string>();
        }

        public async Task<Stream> GenerateCSV(ICSVFormatService format, string accountId, DateTime startDate, DateTime endDate)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                CSVWriteLine writeLine = (string line) => writer.WriteLine(line);
                format.WriteHeader(writeLine);
                var blockIds = await _graphqlService.GetBlockDateRange(startDate, endDate);
                string? lastId = null;
                object? whereObject = null;
                while (true)
                {
                    const int chunkSize = 1000;
                    whereObject = (lastId == null) ?
                        new { block_gte = blockIds!.Item1.ToString(), block_lte = blockIds!.Item2.ToString() } :
                        new { block_gte = blockIds!.Item1.ToString(), block_lte = blockIds!.Item2.ToString(), internalID_lt = lastId };
                    IList<Transaction>? transactions = await _graphqlService.GetAccountTransactions(0, chunkSize, accountId, whereObject)!;
                    if ((transactions == null) || (transactions.Count == 0))
                    {
                        if (lastId == null)
                            throw new Exception("No transactions found in the given timespan");
                        break;
                    }
                    foreach (var transaction in transactions)
                    {
                        format.WriteTransaction(transaction, accountId, writeLine);
                    }
                    if (transactions.Count < chunkSize) break;
                    lastId = transactions.Last().internalID;
                }
            }
            stream.Position = 0;
            return stream;
        }
    }
}
