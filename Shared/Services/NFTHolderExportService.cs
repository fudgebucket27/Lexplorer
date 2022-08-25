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
    public interface ICSVNFTHolderFormatService
    {
        public void SuggestFileName(ref string fileName);
        public void WriteHeader(CSVWriteLine writeLine);
        public void WriteHolder(string nftId, string address, double balance, CSVWriteLine writeLine);
    }

    public class NFTHolderExportService
    {
        private static readonly Dictionary<string, ICSVNFTHolderFormatService> registeredExportServices;

        static NFTHolderExportService()
        {
            registeredExportServices = new Dictionary<string, ICSVNFTHolderFormatService>();
        }

        public static void RegisterExportService(string CSVFormatName, ICSVNFTHolderFormatService service)
        {
            registeredExportServices.Add(CSVFormatName, service);
        }

        private readonly LoopringGraphQLService _graphqlService;

        public NFTHolderExportService(LoopringGraphQLService graphQLService)
        {
            _graphqlService = graphQLService;
        }

        public ICSVNFTHolderFormatService getFormatService(string CSVFormat)
        {
            return registeredExportServices[CSVFormat];
        }

        public List<string> ExportFormats()
        {
            return registeredExportServices.Keys.ToList<string>();
        }

        public async Task<Stream> GenerateCSV(ICSVNFTHolderFormatService format, string nftId)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                CSVWriteLine writeLine = (string line) => writer.WriteLine(line);
                format.WriteHeader(writeLine);
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
                        format.WriteHolder(nftId, holder.account!.address!, holder.balance, writeLine);
                    }
                    if (holders.Count < chunkSize) break;
                    processed += chunkSize;
                }
            }
            stream.Position = 0;
            return stream;
        }
    }
}
