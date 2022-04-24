using LazyCache;
using Lexplorer.Helpers;
using Lexplorer.Models;
using Lexplorer.Services;
using Microsoft.AspNetCore.Components;

namespace Lexplorer.Pages;
public partial class PairsOverview : ComponentBase
{
    [Inject] ILoopringGraphQLService LoopringGraphQLService { get; set; }
    [Inject] IUniswapGraphQLService UniswapGraphQLService { get; set; }
    [Inject] IAppCache AppCache { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string pageNumber { get; set; } = "0";

    private Pairs? pairsData;
    private List<UniswapToken>? uniswapTokens;
    private bool isLoading;

    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrEmpty(pageNumber))
        {
            pageNumber = "0";
        }
        isLoading = true;
        string pairsCacheKey = $"pairsOverview-pairs-page{pageNumber}";
        pairsData = await AppCache.GetOrAddAsyncNonNull(pairsCacheKey, async () => await LoopringGraphQLService.GetPairs(Int32.Parse(pageNumber) * 10), DateTimeOffset.UtcNow.AddHours(1));
        int pairCount = 0;
        foreach (var pair in pairsData!.data!.pairs!)
        {
            string uniswapTokenCacheKey = $"pairsOverview-token-{pair!.token1!.address!}-pageNumber{pageNumber}";
            var uniswapToken = await AppCache.GetOrAddAsyncNonNull(uniswapTokenCacheKey, async () => await UniswapGraphQLService.GetTokenPrice(pair!.token1!.address!), DateTimeOffset.UtcNow.AddHours(1));
            if (uniswapToken != null && pairCount == 0)
            {
                uniswapTokens = new List<UniswapToken>();
                uniswapTokens!.Add(uniswapToken!);
            }
            else
            {
                uniswapTokens!.Add(uniswapToken!);
            }

            pairCount++;
        }
        isLoading = false;
        StateHasChanged();
    }

    private void GoToNextPage()
    {
        int nextPage = Int32.Parse(pageNumber) + 1;
        string parameters = $"pairs?pageNumber={nextPage.ToString()}";
        NavigationManager.NavigateTo(parameters);
    }

    private void GoToPreviousPage()
    {
        int previousPage = Int32.Parse(pageNumber) - 1;
        string parameters = $"pairs?pageNumber={previousPage.ToString()}";
        NavigationManager.NavigateTo(parameters);
    }
    private void GoToStartPage()
    {
        string parameters = "pairs/";
        NavigationManager.NavigateTo(parameters);
    }
}
