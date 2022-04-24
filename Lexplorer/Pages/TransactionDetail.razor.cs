using LazyCache;
using Lexplorer.Helpers;
using Lexplorer.Models;
using Lexplorer.Services;
using Microsoft.AspNetCore.Components;

namespace Lexplorer.Pages;
public partial class TransactionDetail : ComponentBase
{
    [Inject] ILoopringGraphQLService LoopringGraphQLService { get; set; }
    [Inject] IAppCache AppCache { get; set; }

    [Parameter]
    public string transactionId { get; set; } = "";

    private Transaction? transaction;
    private Swap? swap { get { return transaction as Swap; } }
    private Transfer? transfer { get { return transaction as Transfer; } }
    private OrderBookTrade? orderBookTrade { get { return transaction as OrderBookTrade; } }
    private Deposit? deposit { get { return transaction as Deposit; } }
    private Withdrawal? withdrawal { get { return transaction as Withdrawal; } }
    private Add? add { get { return transaction as Add; } }
    private Remove? remove { get { return transaction as Remove; } }
    private AmmUpdate? ammUpdate { get { return transaction as AmmUpdate; } }
    private MintNFT? mintNFT { get { return transaction as MintNFT; } }
    private TransferNFT? transferNFT { get { return transaction as TransferNFT; } }
    public WithdrawalNFT? withdrawalNFT { get { return transaction as WithdrawalNFT; } }
    public TradeNFT? tradeNFT { get { return transaction as TradeNFT; } }

    protected override async Task OnParametersSetAsync()
    {
        string transactionCacheKey = $"transactionDetail-{transactionId}";
        if (transaction?.id != transactionId)
        {
            transaction = await AppCache.GetOrAddAsyncNonNull(transactionCacheKey, async () => await LoopringGraphQLService.GetTransaction(transactionId));
            StateHasChanged();
        }
    }
}
