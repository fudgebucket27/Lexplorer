using LazyCache;
using Lexplorer.Components;
using Lexplorer.Helpers;
using Lexplorer.Models;
using Lexplorer.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Diagnostics;

namespace Lexplorer.Pages;
public partial class AccountDetail : ComponentBase
{
    [Inject] ILoopringGraphQLService LoopringGraphQLService { get; set; }
    [Inject] IEthereumService EthereumService { get; set; }
    [Inject] INftMetadataService NftMetadataService { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }
    [Inject] IDialogService DialogService { get; set; }
    [Inject] IAppCache AppCache { get; set; }

    [Parameter]
    public string? accountId { get; set; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string pageNumber { get; set; } = "1";

    [Parameter]
    [SupplyParameterFromQuery]
    public string nftPageNumber { get; set; } = "1";

    public int gotoNFTPage
    {
        get
        {
            return int.TryParse(nftPageNumber, out int np) ? np : 1;
        }
        set
        {
            string URL = $"/account/{accountId}?pageNumber={pageNumber}&nftPageNumber={value}";
            NavigationManager.NavigateTo(URL);
        }
    }
    public int gotoPage
    {
        get
        {
            return int.TryParse(pageNumber, out int np) ? np : 1;
        }
        set
        {
            string URL = $"/account/{accountId}?pageNumber={value}&nftPageNumber={nftPageNumber}";
            NavigationManager.NavigateTo(URL);
        }
    }
    public bool balancesLoading;
    public bool isLoading = true;
    public readonly int pageSize = 25;
    public readonly int nftPageSize = 12; //6 per row

    private Models.Account? account { get; set; }
    private IList<Transaction>? transactions { get; set; } = new List<Transaction>();
    private IList<AccountNFTSlot>? accountNFTSlots { get; set; }
    private Dictionary<string, NftMetadata> NFTdata { get; set; } = new Dictionary<string, NftMetadata>();
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
                //did the account change?
                if (account != null && account.id != accountId)
                {
                    account = null;
                    transactions = new List<Transaction>();
                    accountNFTSlots = null;
                    pageNumber = "1";
                    nftPageNumber = "1";
                    StateHasChanged();
                }
                if (accountId == null) return;
                if (account == null)
                {
                    balancesLoading = true;
                    account = await LoopringGraphQLService.GetAccount(accountId, localCTS.Token);
                    localCTS.Token.ThrowIfCancellationRequested();
                    if (account == null) return;
                    StateHasChanged();
                    account!.balances = await LoopringGraphQLService.GetAccountBalance(accountId, localCTS.Token);
                    localCTS.Token.ThrowIfCancellationRequested();
                    balancesLoading = false;
                    StateHasChanged();
                }
                if (String.IsNullOrEmpty(pageNumber))
                {
                    pageNumber = "1";
                }

                string transactionCacheKey = $"account{accountId}-transactions-page{pageNumber}";
                transactions = await AppCache.GetOrAddAsyncNonNull(transactionCacheKey,
                    async () => await LoopringGraphQLService.GetAccountTransactions((gotoPage - 1) * pageSize, pageSize, accountId, cancellationToken: localCTS.Token),
                    DateTimeOffset.UtcNow.AddMinutes(10));
                localCTS.Token.ThrowIfCancellationRequested();
                StateHasChanged();

                string nftCacheKey = $"account{accountId}-nftSlots-page{nftPageNumber}";
                accountNFTSlots = await AppCache.GetOrAddAsyncNonNull(nftCacheKey,
                    async () => await LoopringGraphQLService.GetAccountNFTs((gotoNFTPage - 1) * nftPageSize, nftPageSize, accountId, localCTS.Token),
                    DateTimeOffset.UtcNow.AddMinutes(10));
                localCTS.Token.ThrowIfCancellationRequested();

                Dictionary<string, NftMetadata> localNFTdata = new Dictionary<string, NftMetadata>();
                NFTdata = localNFTdata;

                if (accountNFTSlots != null)
                {
                    StateHasChanged();
                    foreach (var slot in accountNFTSlots!)
                    {
                        string nftMetadataLinkCacheKey = $"nftMetadataLink-{slot.nft!.nftID}";
                        string? nftMetadataLink = await AppCache.GetOrAddAsyncNonNull(nftMetadataLinkCacheKey,
                            async () => await EthereumService.GetMetadataLink(slot.nft?.nftID, slot.nft?.token, slot.nft?.nftType),
                            DateTimeOffset.UtcNow.AddHours(1));
                        if (String.IsNullOrEmpty(nftMetadataLink)) continue;

                        string nftMetadataCacheKey = $"nftMetadata-{nftMetadataLink}";
                        var nftMetadata = await AppCache.GetOrAddAsyncNonNull(nftMetadataCacheKey,
                            async () => await NftMetadataService.GetMetadata(nftMetadataLink, localCTS.Token),
                            DateTimeOffset.UtcNow.AddHours(1));
                        localCTS.Token.ThrowIfCancellationRequested();
                        if (nftMetadata == null) continue;

                        localNFTdata.Add(slot.nft!.id!, nftMetadata);
                        StateHasChanged();
                    }
                }
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

    private NftMetadata? GetMetadata(string? nftID)
    {
        NftMetadata? data = null;
        if (nftID != null)
        {
            NFTdata.TryGetValue(nftID, out data);
        }
        return data;
    }

    private void ShowCSVOptions()
    {
        var parameters = new DialogParameters();
        parameters.Add("accountId", accountId);

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };

        DialogService.Show<TransactionExportDialog>("Export", parameters, options);
    }
}
