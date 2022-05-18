using System;
using System.Collections.Generic;
using System.Text;

namespace Lexplorer.Models;

public class LoopStatsDailyCount
{
    public long BlockCount { get; set; }
    public long TransactionCount { get; set; }
    public long TransferCount { get; set; }
    public long TransferNFTCount { get; set; }
    public long TradeNFTCount { get; set; }
    public long NFTMintCount { get; set; }
    public long UserCount { get; set; }
    public long NFTCount { get; set; }
}
