using Lexplorer.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Lexplorer.Components;
public partial class TransactionExportDialog : ComponentBase
{
    [Inject] ITransactionExportService TransactionExportService { get; set; }
    [Inject] IJSRuntime JS { get; set; }

    [CascadingParameter]
    MudDialogInstance? MudDialog { get; set; }

    [Parameter]
    public string accountId { get; set; } = "";

    public DateTime? startDate = new DateTime(DateTime.Today.Year, 1, 1);
    public DateTime? endDate = DateTime.Today;
    public string? CSVType = "Default";
    public bool downloadProcessing = false;
    public string? errorMessage = null;

    /// <summary>
    /// / download relying on JavaScript (in _Host.cshtml) as outlined here
    /// / https://docs.microsoft.com/en-us/answers/questions/243420/blazor-server-app-downlaod-files-from-server.html
    /// / since we're not on a page and don't have a corresponding PageModel, other approaches likes these could not be used
    /// / https://swimburger.net/blog/dotnet/create-zip-files-on-http-request-without-intermediate-files-using-aspdotnet-mvc-razor-pages-and-endpoints#better-mvc
    /// / https://stackoverflow.com/questions/59596338/how-to-download-in-memory-file-from-blazor-server-side
    /// / https://stackoverflow.com/questions/67880262/save-razor-component-as-jpg-png-pdf-via-download-link/69350066#69350066
    /// / https://stackoverflow.com/questions/62086035/how-to-dynamically-generate-file-for-download-in-razor-pages
    /// </summary>
    /// 
    private async Task DownloadFileFromStream()
    {
        downloadProcessing = true;
        errorMessage = null;

        try
        {
            //ensure start of day / end of day
            DateTime myStart = startDate!.Value.Date;
            DateTime myEnd = endDate!.Value.Date.AddDays(1).AddTicks(-1);

            var format = TransactionExportService.getFormatService(CSVType!);

            var fileName = $"Transactions_{accountId}.txt";
            format.SuggestFileName(ref fileName, accountId, myStart, myEnd);

            var fileStream = await TransactionExportService.GenerateCSV(format, accountId,
                myStart, myEnd);

            using var streamRef = new DotNetStreamReference(stream: fileStream);

            await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);

            MudDialog!.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            downloadProcessing = false;
        }
    }

    void Cancel() => MudDialog!.Cancel();
}
