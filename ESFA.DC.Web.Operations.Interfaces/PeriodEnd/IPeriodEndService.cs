using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Models.Summarisation;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IPeriodEndService
    {
        Task InitialisePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken);

        Task StartPeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task ProceedAsync(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetPathItemStatesAsync(int? year, int? period, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task CollectionClosedEmailSentAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task ToggleReferenceDataJobsAsync(int year, int period, bool pause, CancellationToken cancellationToken = default(CancellationToken));

        Task PublishProviderReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task PublishMcaReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> ClosePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task ReSubmitFailedJobAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReportDetails>> GetPeriodEndReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReportDetails>> GetSampleReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReportDetails>> GetMcaReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<CollectionStats>> GetCollectionStatsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetPrepStateAsync(int? year, int? period, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<SummarisationCollectionReturnCode>> GetLatestSummarisationCollectionCodesAsync(string collectionType, int numberOfPeriods, CancellationToken cancellationToken);

        Task<List<SummarisationCollectionReturnCode>> GetSummarisationCollectionCodesAsync(string collectionType, int year, int period, CancellationToken cancellationToken);

        Task<List<SummarisationTotal>> GetSummarisationTotalsAsync(List<int> collectionReturnIds, CancellationToken cancellationToken);
    }
}