using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Models.Summarisation;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IPeriodEndService
    {
        Task InitialisePeriodEnd(int year, int period, string collectionType, CancellationToken cancellationToken);

        Task StartPeriodEnd(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task Proceed(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetPathItemStates(int? year, int? period, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task CollectionClosedEmailSent(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task ToggleReferenceDataJobs(int year, int period, bool pause, CancellationToken cancellationToken = default(CancellationToken));

        Task PublishProviderReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task PublishMcaReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task ClosePeriodEnd(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task ReSubmitFailedJob(long jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReportDetails>> GetPeriodEndReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReportDetails>> GetSampleReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReportDetails>> GetMcaReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<CollectionStats>> GetCollectionStatsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetPrepState(int? year, int? period, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<SummarisationCollectionReturnCode>> GetLatestSummarisationCollectionCodesAsync(string collectionType, int numberOfPeriods, CancellationToken cancellationToken);

        Task<List<SummarisationTotal>> GetSummarisationTotalsAsync(List<int> collectionReturnIds, CancellationToken cancellationToken);
    }
}