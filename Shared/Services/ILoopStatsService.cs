using Lexplorer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Lexplorer.Services;

public interface ILoopStatsService
{
    void Dispose();
    Task<LoopStatsDailyCount> GetDailyCount(CancellationToken cancellationToken = default);
}
