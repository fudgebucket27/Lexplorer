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

        public async Task<Stream> GenerateCSV(string nftId, NonFungibleToken? nft)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                CSVWriteLine writeLine = (string line) => writer.WriteLine(line);
                //write header
                if (nft != null)
                {
                    DoWriteLine(writeLine, "NftId", nft.nftID);
                    DoWriteLine(writeLine, "NftType", nft.nftTypeName);
                    DoWriteLine(writeLine, "Token", nft.token);
                    DoWriteLine(writeLine, "Minter", nft.minter?.id);
                    DoWriteLine(writeLine, "MintedAt", nft.mintedAtTransaction?.id);
                    DoWriteLine(writeLine, "MintedAmount", nft.mintedAtTransaction?.amount.ToString());
                    writeLine("");
                }
                DoWriteLine(writeLine, "Account", "Address", "Balance");
                string? lastSlotID = null;
                while (true)
                {
                    const int chunkSize = 200;
                    IList<AccountNFTSlot>? holders = await _graphqlService.GetNftHolders(nftId, 0, chunkSize, "id", "asc",
                        lastSlotID == null ? null : new { id_gt = lastSlotID })!;
                    if ((holders == null) || (holders.Count == 0))
                    {
                        if (lastSlotID == null)
                            throw new Exception("No holders found!");
                        break;
                    }
                    foreach (var holder in holders)
                    {
                        DoWriteLine(writeLine, holder.account!.id, holder.account!.address!, holder.balance.ToString());
                    }
                    if (holders.Count < chunkSize) break;
                    lastSlotID = holders.Last().id;
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
