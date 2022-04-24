using LazyCache;
using Lexplorer.Helpers;
using Lexplorer.Models;
using Lexplorer.Services;
using Microsoft.AspNetCore.Components;

namespace Lexplorer.Pages;
public partial class NFTOverview : ComponentBase
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

    public bool isLoading = true;
    public readonly int pageSize = 25;

    private IList<NonFungibleToken>? nfts { get; set; } = new List<NonFungibleToken>();

    protected override async Task OnParametersSetAsync()
    {
        isLoading = true;

        if (String.IsNullOrEmpty(pageNumber))
        {
            pageNumber = "1";
        }

        string nftsCacheKey = $"nfts-page{pageNumber}";
        nfts = await AppCache.GetOrAddAsyncNonNull(nftsCacheKey,
            async () => await LoopringGraphQLService.GetNFTs((gotoPage - 1) * pageSize, pageSize),
            DateTimeOffset.UtcNow.AddMinutes(10));
        isLoading = false;
        StateHasChanged();
    }

    private void navigateTo(int page)
    {
        string URL = $"/nfts?pageNumber={page}";
        NavigationManager.NavigateTo(URL);
    }
}
