using Lexplorer.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexplorer.Services
{
    public class NFTHolderExportDefaultCSVFormat : ICSVNFTHolderFormatService
    {
        protected readonly StringBuilder sb = new StringBuilder();
        public virtual void SuggestFileName(ref string fileName)
        {
            //leave fileName unchanged
        }

        public virtual void BuildLine(params string?[] columns)
        {
            sb.AppendJoin(Convert.ToChar(9), columns);
        }

        public virtual void WriteHeader(CSVWriteLine writeLine)
        {
            sb.Clear();
            BuildLine("NftId", "Address", "Balance");
            writeLine(sb.ToString());
        }

        public void WriteHolder(string nftId, string address, double balance, CSVWriteLine writeLine)
        {
            sb.Clear();
            sb.AppendJoin(Convert.ToChar(9), nftId, address, balance);
            writeLine(sb.ToString());
        }
    }
}
