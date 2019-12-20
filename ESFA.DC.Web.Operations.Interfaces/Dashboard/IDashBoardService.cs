using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Dashboard
{
    public interface IDashBoardService
    {
        Task<string> GetStatsAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}