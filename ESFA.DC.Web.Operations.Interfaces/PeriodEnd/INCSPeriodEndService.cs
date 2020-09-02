using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface INCSPeriodEndService
    {
        Task InitialisePeriodEndAsync(int year, int period, CancellationToken cancellationToken);

        Task StartPeriodEndAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task ProceedAsync(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetPathItemStatesAsync(int? year, int? period, CancellationToken cancellationToken = default(CancellationToken));

        Task CollectionClosedEmailSentAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task ClosePeriodEndAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task ReSubmitFailedJobAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReportDetails>> GetPeriodEndReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetPrepStateAsync(int? year, int? period, CancellationToken cancellationToken = default(CancellationToken));
    }
}