using System;
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
    public class NFTHolderExportService
    {
        private readonly LoopringGraphQLService _graphqlService;

        public NFTHolderExportService(LoopringGraphQLService graphQLService)
        {
            _graphqlService = graphQLService;
        }

        public async Task<Stream> GenerateCSV(string nftId)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                CSVWriteLine writeLine = (string line) => writer.WriteLine(line);
                DoWriteLine(writeLine, "NftId", "Address", "Balance");
                var processed = 0;
                while (true)
                {
                    const int chunkSize = 10;
                    IList<AccountNFTSlot>? holders = await _graphqlService.GetNftHolders(nftId,processed, chunkSize)!;
                    if ((holders == null) || (holders.Count == 0))
                    {
                        if (processed == 0)
                            throw new Exception("No holders found!");
                        break;
                    }
                    foreach (var holder in holders)
                    {
                        DoWriteLine(writeLine, nftId, holder.account!.address!, holder.balance.ToString());
                    }
                    if (holders.Count < chunkSize) break;
                    processed += chunkSize;
                }
            }
            stream.Position = 0;
            return stream;
        }

        private readonly StringBuilder sb = new StringBuilder();

        private void DoWriteLine(CSVWriteLine writeLine, params string?[] columns)
        {
            sb.Clear();
            sb.AppendJoin(Convert.ToChar(9), columns);
            writeLine(sb.ToString());
        }

    }
}
