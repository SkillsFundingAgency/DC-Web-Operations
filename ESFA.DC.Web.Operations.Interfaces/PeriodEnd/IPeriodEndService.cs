using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.PeriodEnd;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IPeriodEndService
    {
        Task StartPeriodEnd(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task Proceed(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetPathItemStates(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task ToggleReferenceDataJobs(bool pause, CancellationToken cancellationToken = default(CancellationToken));

        Task PublishReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task ClosePeriodEnd(int year, int period, CancellationToken cancellationToken = default(CancellationToken));
    }
}