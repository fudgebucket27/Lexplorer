using LazyCache;
using Lexplorer.Helpers;
using Lexplorer.Models;
using Lexplorer.Services;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace Lexplorer.Pages;
public partial class BlockDetails : ComponentBase
{
    [Inject] ILoopringGraphQLService LoopringGraphQLService { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }
    [Inject] IAppCache AppCache { get; set; }

    [Parameter]
    public string blockNumber { get; set; } = "";

    [Parameter]
    [SupplyParameterFromQuery]
    public string pageNumber { get; set; } = "1";

    private Block? block;
    private List<Transaction>? transactions = new List<Transaction>();
    public bool isTransactionLoading = true;
    public bool isBlockLoading = true;
    private int pageSize = 15;
    private CancellationTokenSource? cts;

    public int blockId
    {
        get
        {
            return Math.Max(1, Int32.Parse(blockNumber ?? "1"));
        }
        set
        {
            navigateTo(value, 1);
        }
    }

    public int gotoPage
    {
        get
        {
            return Math.Max(1, Int32.Parse(pageNumber ?? "1"));
        }
        set
        {
            navigateTo(blockId, value);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        //cancel any previous OnParametersSetAsync which might still be running
        cts?.Cancel();

        using (CancellationTokenSource localCTS = new CancellationTokenSource())
        {
            //give future calls a chance to cancel us; it is now safe to replace
            //any previous value of cts, since we already cancelled it above
            try
            {

                cts = localCTS;

                isBlockLoading = true;
                string blockCacheKey = $"blockDetailOverview-{blockId}";
                block = await AppCache.GetOrAddAsyncNonNull(blockCacheKey,
                    async () => await LoopringGraphQLService.GetBlockDetails(blockId, localCTS.Token),
                    DateTimeOffset.UtcNow.AddMinutes(5));
                if (block == null) return;
                isBlockLoading = false;
                StateHasChanged();

                isTransactionLoading = true;
                string transactionDatacacheKey = $"blockDetailTransactions-{blockId}-page{gotoPage}";
                var transactionData = await AppCache.GetOrAddAsyncNonNull(transactionDatacacheKey,
                    async () => await LoopringGraphQLService.GetTransactions((gotoPage - 1) * pageSize, pageSize, blockId: blockNumber, cancellationToken: localCTS.Token),
                    DateTimeOffset.UtcNow.AddMinutes(5));
                transactions = transactionData?.data?.transactions ?? new List<Transaction>();
                isTransactionLoading = false;
                StateHasChanged();

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
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

    private void navigateTo(int block, int page)
    {
        string URL = $"blocks/{block}?pageNumber={page}";
        NavigationManager.NavigateTo(URL);
    }
}
