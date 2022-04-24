using LazyCache;
using Lexplorer.Helpers;
using Lexplorer.Models;
using Lexplorer.Services;
using Microsoft.AspNetCore.Components;

namespace Lexplorer.Pages;
public partial class TransactionsOverview : ComponentBase
{
    [Inject] ILoopringGraphQLService LoopringGraphQLService { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }
    [Inject] IAppCache AppCache { get; set; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string pageNumber { get; set; } = "1";

    [Parameter]
    [SupplyParameterFromQuery]
    public string? type { get; set; }

    public bool isLoading = true;
    public readonly int pageSize = 25;
    private IList<Transaction>? transactions { get; set; } = new List<Transaction>();

    public int gotoPage
    {
        get
        {
            return Int32.Parse(pageNumber ?? "1");
        }
        set
        {
            navigateTo(value);
        }
    }

    public string? filterTransaction
    {
        get
        {
            return type ?? "All";
        }
        set
        {
            type = (string.Equals(value ?? "All", "All", StringComparison.InvariantCultureIgnoreCase)) ? null : value;
            navigateTo(gotoPage);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        isLoading = true;

        if (String.IsNullOrEmpty(pageNumber))
        {
            pageNumber = "1";
        }

        string transactionCacheKey = $"transactions-page{pageNumber}-type{type}";
        var transactionsData = await AppCache.GetOrAddAsyncNonNull(transactionCacheKey,
            async () => await LoopringGraphQLService.GetTransactions((gotoPage - 1) * pageSize, pageSize, typeName: type),
            DateTimeOffset.UtcNow.AddMinutes(10));
        transactions = transactionsData?.data?.transactions;
        isLoading = false;
        StateHasChanged();
    }

    private void navigateTo(int page)
    {
        string URL = $"/transactions?pageNumber={page}";
        if (type != null)
            URL += $"&type={type}";
        NavigationManager.NavigateTo(URL);
    }
}
