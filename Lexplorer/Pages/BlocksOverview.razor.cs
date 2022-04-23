using LazyCache;
using Lexplorer.Helpers;
using Lexplorer.Models;
using Lexplorer.Services;
using Microsoft.AspNetCore.Components;

namespace Lexplorer.Pages;
public partial class BlocksOverview : ComponentBase
{
    [Inject] ILoopringGraphQLService LoopringGraphQLService { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }
    [Inject] IAppCache AppCache { get; set; }

    private List<BlockDetail>? blocks = new List<BlockDetail>();

    [Parameter]
    [SupplyParameterFromQuery]
    public string pageNumber { get; set; } = "1";

    public int gotoPage
    {
        get
        {
            return Math.Max(1, Int32.Parse(pageNumber ?? "1"));
        }
        set
        {
            string URL = $"/blocks?pageNumber={value}";
            NavigationManager.NavigateTo(URL);
        }
    }


    public bool isLoading = true;
    public readonly int pageSize = 25;

    protected override async Task OnParametersSetAsync()
    {
        isLoading = true;
        string blockCacheKey = $"blocks-{gotoPage}";
        var blockData = await AppCache.GetOrAddAsyncNonNull(blockCacheKey,
            async () => await LoopringGraphQLService.GetBlocks((gotoPage - 1) * pageSize, pageSize),
            DateTimeOffset.UtcNow.AddMinutes(10));
        blocks = blockData?.data?.blocks ?? new List<BlockDetail>();
        isLoading = false;
        StateHasChanged();
    }
}
