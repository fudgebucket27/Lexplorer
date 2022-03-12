namespace Lexplorer.Helpers
{
    using Microsoft.AspNetCore.Components;
    using Lexplorer.Models;

    public class LinkHelper
    {
        public static MarkupString CreateUserLink(Account? account, string? ignoreId = null, bool includeAccountId = false)
        {
            return CreateUserLink(account?.id, account?.address, true, ignoreId, includeAccountId);
        }
        public static MarkupString CreateUserLink(Account? account, bool shortenAddress = true, bool includeAccountId = false)
        {
            return CreateUserLink(account?.id, account?.address, shortenAddress, null, includeAccountId);
        }

        public static MarkupString CreateUserLink(string? id, string? address, bool shortenAddress = true, string?
            ignoreId = null, bool includeAccountId = false)
        {
            if (address == null) return new MarkupString();

            String link = shortenAddress ? String.Format("{0}...{1}", address.Substring(0, 5), address.Substring(address.Length - 6, 6)) : address;
            if (includeAccountId)
                link += $" ({id})";
            if ((id != null) && ((ignoreId == null) || (id != ignoreId)))
            {
                link = String.Format(@"<a Class=""mud-theme-primary"" href=""account/{0}"">{1}</a>", id, link);
            }
            return new MarkupString(link);
        }

        public static Tuple<string, string>? GetObjectLinkAddress(object? linkedObject)
        {
            if (linkedObject is Account)
                return new Tuple<string, string>($"account/{((Account)linkedObject).id}", ((Account)linkedObject).id ?? "");
            else if (linkedObject is BlockDetail)
                return new Tuple<string, string>($"blocks/{((BlockDetail)linkedObject).id}", ((BlockDetail)linkedObject).id ?? "");
            else if (linkedObject is Transaction)
                return new Tuple<string, string>($"transactions/{((Transaction)linkedObject).id}", 
                    ((Transaction)linkedObject).id ?? "");
            else
                return null;
        }

        public static MarkupString GetObjectLink(object? linkedObject)
        {
            Tuple<string, string>? adr = GetObjectLinkAddress(linkedObject);
            if (adr == null) return new MarkupString();
            return new MarkupString($"<a Class=\"mud-theme-primary\" href=\"{adr.Item1}\">{adr.Item2}</a>");
        }
    }
}
