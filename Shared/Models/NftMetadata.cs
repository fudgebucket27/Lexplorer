using System;
using System.Collections.Generic;
using System.Text;

namespace Lexplorer.Models
{
    public class NftMetadata
    {
        public string? description { get; set;}
        public string? image { get; set; }
        public string? imageURL
        {
            get
            {
                if (image == null) return null;
                //remove the ipfs:// when concatenating with mypinata URL
                return image.StartsWith("ipfs://") ? string.Concat("https://fudgey.mypinata.cloud/ipfs/", image.Remove(0, 7)) : image;
            }
        }
        public string? name { get; set; }
        public int royalty_percentage { get; set; }

        public string? animation_url { get; set; }

        public string? animationURL
        {
            get
            {
                if (animation_url == null) return null;
                //remove the ipfs:// when concatenating with mypinata URL
                return animation_url.StartsWith("ipfs://") ? string.Concat("https://fudgey.mypinata.cloud/ipfs/", animation_url.Remove(0, 7)) : animation_url;
            }
        }

        public string? contentType { get; set; }
    }
}
