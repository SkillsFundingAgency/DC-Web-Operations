using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IPeriodEndService
    {
        Task StartPeriodEnd(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task Proceed(int startIndex = 0, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetPathItemStates(CancellationToken cancellationToken);
    }
}