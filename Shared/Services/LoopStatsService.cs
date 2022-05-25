using Lexplorer.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Diagnostics;
using System;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Net;
using Lexplorer.Exceptions;

namespace Lexplorer.Services;

public class LoopStatsService : ILoopStatsService, IDisposable
{
    private readonly RestClient _restClient;

    public LoopStatsService(IConfiguration config)
    {
        var baseUrl = config.GetSection("services:loopstats:endpoint").Value;
        _restClient = new RestClient(baseUrl);
    }

    public async Task<LoopStatsDailyCount> GetDailyCount(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _restClient.GetJsonAsync<LoopStatsDailyCount>("api/GetLastDaysCount", cancellationToken);

            return response;
        }
        catch (HttpRequestException exc)
        {
            if (exc.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException(nameof(LoopStatsDailyCount), innerException: exc.InnerException);
            }

            throw;
        }
    }
    public void Dispose()
    {
        _restClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}
