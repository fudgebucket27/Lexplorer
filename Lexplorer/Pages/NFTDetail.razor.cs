using LazyCache;
using Lexplorer.Helpers;
using Lexplorer.Models;
using Lexplorer.Services;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace Lexplorer.Pages;
public partial class NFTDetail : ComponentBase
{
    [Inject] ILoopringGraphQLService LoopringGraphQLService { get; set; }
    [Inject] IEthereumService EthereumService { get; set; }
    [Inject] INftMetadataService NftMetadataService { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }
    [Inject] IAppCache AppCache { get; set; }

    [Parameter]
    public string nftId { get; set; } = "";

    [Parameter]
    [SupplyParameterFromQuery]
    public string pageNumber { get; set; } = "1";

    private NonFungibleToken? nft;
    private IList<Lexplorer.Models.Transaction>? transactions { get; set; } = new List<Transaction>();
    public bool isLoading = true;
    public readonly int pageSize = 25;

    private NftMetadata? nftMetadata;

    public int gotoPage
    {
        get
        {
            return Int32.Parse(pageNumber ?? "1");
        }
        set
        {
            string URL = $"/nfts/{nftId}?pageNumber={value}";
            NavigationManager.NavigateTo(URL);
        }
    }

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
                if (nft != null && nft.id != nftId) //did we change to another NFT?
                {
                    nft = null;
                    transactions = new List<Transaction>();
                    pageNumber = "1";
                    StateHasChanged();
                }
                if (nftId == null) return;
                if (nft == null)
                {
                    nftMetadata = null;
                    string nftCacheKey = $"nft-{nftId}";
                    nft = await AppCache.GetOrAddAsyncNonNull(nftCacheKey,
                        async () => await LoopringGraphQLService.GetNFT(nftId, localCTS.Token),
                        DateTimeOffset.UtcNow.AddHours(1));
                    if (nft == null) return;
                    StateHasChanged();
                }

                if (String.IsNullOrEmpty(pageNumber))
                {
                    pageNumber = "1";
                }

                string nftTransactionCacheKey = $"nft{nftId}-transactions-page{pageNumber}";
                transactions = await AppCache.GetOrAddAsyncNonNull(nftTransactionCacheKey,
                    async () => await LoopringGraphQLService.GetNFTTransactions((gotoPage - 1) * pageSize, pageSize, nftId, localCTS.Token),
                    DateTimeOffset.UtcNow.AddMinutes(10));
                isLoading = false;
                StateHasChanged();

                //load the NFT metadata last - it might actually take the longest and other services are queried etc.
                if (nftMetadata == null)
                {
                    string nftMetadataLinkCacheKey = $"nftMetadataLink-{nft?.nftID}";
                    string? nftMetadataLink = await AppCache.GetOrAddAsyncNonNull(nftMetadataLinkCacheKey,
                        async () => await EthereumService.GetMetadataLink(nft?.nftID, nft?.token, nft?.nftType));
                    if (String.IsNullOrEmpty(nftMetadataLink)) return;

                    string nftMetadataCacheKey = $"nftMetadata-{nftMetadataLink}";
                    nftMetadata = await AppCache.GetOrAddAsyncNonNull(nftMetadataCacheKey,
                        async () => await NftMetadataService.GetMetadata(nftMetadataLink, localCTS.Token));
                    if (!String.IsNullOrEmpty(nftMetadata?.animation_url))
                    {
                        string nftMetadataContentTypeCacheKey = $"nftMetadata-contentType-{nftMetadataLink}";
                        nftMetadata!.contentType = await AppCache.GetOrAddAsyncNonNull(nftMetadataContentTypeCacheKey,
                            async () => await NftMetadataService.GetContentTypeFromURL(nftMetadata.animationURL!, localCTS.Token));
                    }
                    StateHasChanged();
                }

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
}
