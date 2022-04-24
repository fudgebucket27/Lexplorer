using Lexplorer.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddHubOptions(o =>
{
    o.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
});
builder.Services.AddMudServices();
builder.Services.AddSingleton<ILoopringGraphQLService, LoopringGraphQLService>();
builder.Services.AddSingleton<IUniswapGraphQLService, UniswapGraphQLService>();
builder.Services.AddSingleton<ITransactionExportService, TransactionExportService>();
builder.Services.AddSingleton<IEthereumService, EthereumService>();
builder.Services.AddSingleton<INftMetadataService, NftMetadataService>();
builder.Services.AddLazyCache();

//registration of CSV export formats, no automatic registration possible
//out of the box and extra framework seems overkill
TransactionExportService.RegisterExportService("Default", new TransactionExportDefaultCSVFormat());
TransactionExportService.RegisterExportService("Cointracking", new TransactionExportCointracking());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();