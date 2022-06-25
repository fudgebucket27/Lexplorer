using System;
using System.Collections.Generic;
using System.Text;

namespace Lexplorer.Models
{
    public class NftMetadata
    {
        public string? description { get; set;}
        public string? image { get; set; }
        public string? name { get; set; }
        public int royalty_percentage { get; set; }
        public string? animation_url { get; set; }
        public string? contentType { get; set; }

        public List<NftAttribute>? attributes { get; set; }

        public string? JSONContent { get; set; }
    }

    public class NftAttribute
    {
        public string? trait_type { get; set; }
        public object? value { get; set; } //value can be either string or int
    }
}
