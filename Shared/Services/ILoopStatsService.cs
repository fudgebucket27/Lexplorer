using Lexplorer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Lexplorer.Services;

public interface ILoopStatsService
{
    Task<LoopStatsDailyCount> GetDailyCount(CancellationToken cancellationToken = default);
}
