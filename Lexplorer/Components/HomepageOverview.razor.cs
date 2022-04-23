using LazyCache;
using Lexplorer.Helpers;
using Lexplorer.Models;
using Lexplorer.Services;
using Microsoft.AspNetCore.Components;

namespace Lexplorer.Components;
public partial class HomepageOverview : ComponentBase
{
    [Inject] ILoopringGraphQLService LoopringGraphQLService { get; set; }
    [Inject] IUniswapGraphQLService UniswapGraphQLService { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }
    [Inject] IAppCache AppCache { get; set; }

    private Blocks? blockData;
    private Transactions? transactionData;
    private Pairs? pairsData;
    private List<UniswapToken>? uniswapTokens;

    private double averageBlockTime;
    private long averageTransactions;
    private double lastBlockSubmittedTime;

    protected override async Task OnInitializedAsync()
    {
        string blockCacheKey = $"homepage-block";
        blockData = await AppCache.GetOrAddAsyncNonNull(blockCacheKey, async () => await LoopringGraphQLService.GetBlocks(0, 10), DateTimeOffset.UtcNow.AddMinutes(10));
        CalculateAverageBlockTime();
        CalculateLastBlockSubmitted();
        StateHasChanged();
        string transactionCacheKey = $"homepage-transaction";
        transactionData = await AppCache.GetOrAddAsyncNonNull(transactionCacheKey, async () => await LoopringGraphQLService.GetTransactions(0, 10), DateTimeOffset.UtcNow.AddMinutes(10));
        StateHasChanged();
        string pairsCacheKey = $"homepage-pairs";
        pairsData = await AppCache.GetOrAddAsyncNonNull(pairsCacheKey, async () => await LoopringGraphQLService.GetPairs(), DateTimeOffset.UtcNow.AddHours(1));
        int pairCount = 0;
        foreach (var pair in pairsData!.data!.pairs!)
        {
            string uniSwapTokenCache = $"homepage-uniswapToken-{pair!.token1!.address!}";
            var uniswapToken = await AppCache.GetOrAddAsyncNonNull(uniSwapTokenCache, async () => await UniswapGraphQLService.GetTokenPrice(pair!.token1!.address!), DateTimeOffset.UtcNow.AddHours(1));
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
        StateHasChanged();
    }

    private decimal? priceOfToken(Token token)
    {
        //find token.address in uniswapTokens and get most recent price; if not available, return 1.0 so no conversion
        return uniswapTokens?.Where(c => c.address == @token?.address).FirstOrDefault()?.data?.tokenDayDatas?[0].priceUSD;
    }

    private string? getVolumePair1(Pair pair, Boolean weekly = true)
    {
        if (pair == null) return null;
        decimal? price = (pair.token1 == null) ? null : priceOfToken(pair.token1);
        var volume = weekly
            ? pair.weeklyEntities![0].tradedVolumeToken1Swap
            : pair.dailyEntities![0].tradedVolumeToken1Swap;
        if (price.HasValue)
            return $"${@TokenAmountConverter.ToStringWithExponent(volume, pair.token1?.decimals ?? 0, price.Value)}";
        else
            return $"{pair.token1?.symbol} {@TokenAmountConverter.ToStringWithExponent(volume, pair.token1?.decimals ?? 0, 1)}";
    }

    private string? get24hPercentage(Pair pair)
    {
        if (pair == null) return null;
        return (pair.dailyEntities![0].tradedVolumeToken1Swap * 7 / pair.weeklyEntities![0].tradedVolumeToken1Swap * 100).ToString("N2");
    }

    private void CalculateAverageBlockTime()
    {
        long transactionCount = 0;
        long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        List<long> timeBetweenBlocks = new List<long>();
        foreach (var block in blockData!.data!.blocks!)
        {
            transactionCount += block.transactionCount;
            timeBetweenBlocks.Add(currentTime - Int64.Parse(block.timestamp!));
            currentTime = Int64.Parse(block.timestamp!);
        }
        averageTransactions = transactionCount / blockData.data.blocks.Count;
        averageBlockTime = Math.Floor(timeBetweenBlocks.Average() / 60);
    }

    private void CalculateLastBlockSubmitted()
    {
        long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        long timeSinceLastBlock = currentTime - Int64.Parse(blockData!.data!.blocks![0].timestamp!);
        lastBlockSubmittedTime = Math.Floor((double)timeSinceLastBlock / 60);
    }

    private void GoToBlockOverviewPage()
    {
        string parameters = "blocks";
        NavigationManager.NavigateTo(parameters);
    }

    private void GoToPairOverviewPage()
    {
        string parameters = "pairs";
        NavigationManager.NavigateTo(parameters);
    }

    private void GoToTransactionOverviewPage()
    {
        string parameters = "transactions";
        NavigationManager.NavigateTo(parameters);
    }
}
