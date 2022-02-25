using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lexplorer.Models
{
    public class TokenDayData
    {
        public decimal priceUSD { get; set; }
    }

    public class UniswapTokenData
    {
        public List<TokenDayData>? tokenDayDatas { get; set; }
    }

    public class UniswapToken
    {
        public UniswapTokenData? data { get; set; }
        public string? address { get; set; }
    }
}
