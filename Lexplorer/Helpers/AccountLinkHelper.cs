namespace Lexplorer.Helpers
{
    using Microsoft.AspNetCore.Components;
    using Lexplorer.Models;

    public class AccountLinkHelper
    {
        public static MarkupString CreateUserLink(Account account, string? ignoreId = null)
        {
            return CreateUserLink(account.id!, account.address!, true, ignoreId);
        }
        public static MarkupString CreateUserLink(Account account, bool shortenAddress = true)
        {
            return CreateUserLink(account.id!, account.address!, shortenAddress, null);
        }

        public static MarkupString CreateUserLink(string id, string address, bool shortenAddress = true, string? ignoreId = null)
        {
            String link = shortenAddress ? String.Format("{0}...{1}", address.Substring(0, 5), address.Substring(address.Length - 6, 6)) : address;
            if ((ignoreId == null) || (id != ignoreId))
            {
                link = String.Format(@"<a Class=""mud-theme-primary"" href=""account/{0}"">{1}</a>", id, link);
            }
            return new MarkupString(link);
        }
    }
}
