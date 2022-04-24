using LazyCache;
using Lexplorer.Helpers;
using Lexplorer.Models;
using Lexplorer.Services;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace Lexplorer.Pages;
public partial class AccountsOverview : ComponentBase
{
    [Inject] ILoopringGraphQLService LoopringGraphQLService { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }
    [Inject] IAppCache AppCache { get; set; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string pageNumber { get; set; } = "1";

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
    [Parameter]
    [SupplyParameterFromQuery]
    public string? type { get; set; }

    public string? filterAccounts
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

    public bool isLoading = true;
    public readonly int pageSize = 25;
    private IList<Models.Account>? accounts { get; set; } = new List<Models.Account>();
    private CancellationTokenSource? cts;

    protected override async Task OnParametersSetAsync()
    {

        //cancel any previous OnParametersSetAsync which might still be running
        cts?.Cancel();

        using (CancellationTokenSource localCTS = new CancellationTokenSource())
        {
            //give future calls a chance to cancel us; it is now safe to replace
            //any previous value of cts, since we already cancelled it above
            cts = localCTS;

            try
            {
                isLoading = true;

                string accountCacheKey = $"transactions-page{gotoPage}-type{type}";
                accounts = await AppCache.GetOrAddAsyncNonNull(accountCacheKey,
                    async () => await LoopringGraphQLService.GetAccounts((gotoPage - 1) * pageSize, pageSize, type, localCTS.Token),
                    DateTimeOffset.UtcNow.AddMinutes(10));
                isLoading = false;
                StateHasChanged();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.StackTrace + "\n" + e.Message);
            }
            //now for cleanup, we must clear cts, but only if it is still our localCTS, which we're about to dispose
            //otherwise a new call has already replaced cts with it's own localCTS
            Interlocked.CompareExchange<CancellationTokenSource?>(ref cts, null, localCTS);
        }
    }

    private void navigateTo(int page)
    {
        string URL = $"/account?pageNumber={page}";
        if (type != null)
            URL += $"&type={type}";
        NavigationManager.NavigateTo(URL);
    }
}
