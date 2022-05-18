using Lexplorer.Models;
using Newtonsoft.Json;
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

public interface ILoopStatsService
{
    Task<LoopStatsDailyCount> GetDailyCount(CancellationToken cancellationToken = default);
}

public class LoopStatsService : ILoopStatsService
{
    private readonly HttpClient _client;

    public LoopStatsService(HttpClient client)
    {
        _client = client;
    }

    public async Task<LoopStatsDailyCount> GetDailyCount(CancellationToken cancellationToken = default)
    {
        try
        {
            var dto = await _client.GetFromJsonAsync<LoopStatsDailyCount>("api/GetLastDaysCount", cancellationToken);
            return dto;
        }
        catch (HttpRequestException exc)
        {
            if (exc.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException(nameof(LoopStatsDailyCount));
            }

            throw;
        }
    }
}
