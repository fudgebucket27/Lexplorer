using System;
using System.Collections.Generic;

namespace Lexplorer.Models
{
    public class ENS
	{
        public class Domain
        {
            public string? id { get; set; }
            public string? name { get; set; }
            public string? labelName { get; set; }
            public string? labelHash { get; set; }
            public Account? resolvedAddress { get; set; }
        }

        public class Account
        {
            public string? id { get; set; }
            public List<Domain>? domains { get; set; }
        }

        public enum SourceType
        {
            Lookup,
            ReverseLookup
        }

    }
}
