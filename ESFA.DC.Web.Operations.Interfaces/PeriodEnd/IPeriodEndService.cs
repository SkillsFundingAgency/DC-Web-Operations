using System.Collections.Generic;
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

        Task PublishProviderReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task PublishMcaReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task ClosePeriodEnd(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetReferenceDataJobs(CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetFailedJobs(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task ReSubmitFailedJob(long jobId);

        Task<IEnumerable<ReportDetails>> GetPeriodEndReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReportDetails>> GetSampleReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken));
    }
}