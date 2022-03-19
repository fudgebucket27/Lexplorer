using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class NftMetadata
    {
        public string description { get; set;}
        public string image { get; set; }
        public string name { get; set; }
        public int royalty_percentage { get; set; }
    }
}
