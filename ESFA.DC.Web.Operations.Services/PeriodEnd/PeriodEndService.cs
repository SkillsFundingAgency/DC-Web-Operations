using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models.Summarisation;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class PeriodEndService : BaseHttpClientService, IPeriodEndService
    {
        private readonly string _baseUrl;

        public PeriodEndService(
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task InitialisePeriodEnd(int year, int period, string collectionType, CancellationToken cancellationToken)
        {
            await SendAsync($"{_baseUrl}/api/period-end/{year}/{period}/{collectionType}/initialise", cancellationToken);
        }

        public async Task StartPeriodEnd(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync($"{_baseUrl}/api/period-end/{year}/{period}/{collectionType}/start", cancellationToken);
        }

        public async Task CollectionClosedEmailSent(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync($"{_baseUrl}/api/period-end/{year}/{period}/collection-closed", cancellationToken);
        }

        public async Task Proceed(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync(_baseUrl + $"/api/period-end/{year}/{period}/{path}/proceed", cancellationToken);
        }

        public async Task ToggleReferenceDataJobs(int year, int period, bool pause, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync(_baseUrl + $"/api/period-end/reference-data-jobs/{year}/{period}/{pause}", cancellationToken);
        }

        public async Task PublishProviderReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync(_baseUrl + $"/api/period-end/provider-reports/{year}/{period}/publish", cancellationToken);
        }

        public async Task PublishMcaReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync(_baseUrl + $"/api/period-end/mca-reports/{year}/{period}/publish", cancellationToken);
        }

        public async Task<string> GetPrepState(int? year, int? period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string data = await GetDataAsync(_baseUrl + $"/api/period-end/states-prep/{year}/{period}", cancellationToken);
            return data;
        }

        public async Task<string> GetPathItemStates(int? year, int? period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            string data = await GetDataAsync(_baseUrl + $"/api/period-end/states-main/{year}/{period}/{collectionType}", cancellationToken);
            return data;
        }

        public async Task ClosePeriodEnd(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync(_baseUrl + $"/api/period-end/{year}/{period}/close", cancellationToken);
        }

        public async Task ReSubmitFailedJob(long jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var jobStatusDto = new JobStatusDto(jobId, Convert.ToInt32(JobStatusType.Ready));
            await SendDataAsync($"{_baseUrl}/api/job/{JobStatusType.Ready}", jobStatusDto, cancellationToken);
        }

        public async Task<IEnumerable<ReportDetails>> GetPeriodEndReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_baseUrl}/api/period-end/reports/{year}/{period}";

            var data = await GetAsync<IEnumerable<ReportDetails>>(url, cancellationToken);

            return data;
        }

        public async Task<IEnumerable<ReportDetails>> GetMcaReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_baseUrl}/api/period-end/mca-reports/{year}/{period}";

            var data = await GetAsync<IEnumerable<ReportDetails>>(url, cancellationToken);

            return data;
        }

        public async Task<IEnumerable<CollectionStats>> GetCollectionStatsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_baseUrl}/api/period-end/collectionstats/{year}/{period}";

            var data = await GetAsync<IEnumerable<CollectionStats>>(url, cancellationToken);

            return data;
        }

        public async Task<IEnumerable<ReportDetails>> GetSampleReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_baseUrl}/api/period-end/reports/{year}/{period}/samples";

            var data = await GetAsync<IEnumerable<ReportDetails>>(url, cancellationToken);

            return data;
        }

        public async Task<List<SummarisationCollectionReturnCode>> GetLatestSummarisationCollectionCodesAsync(string collectionType, int numberOfPeriods, CancellationToken cancellationToken)
        {
            string url = $"{_baseUrl}/api/summarisation/return-codes/{collectionType}/{numberOfPeriods}";

            var data = await GetAsync<List<SummarisationCollectionReturnCode>>(url, cancellationToken);

            return data;
        }

        public async Task<List<SummarisationTotal>> GetSummarisationTotalsAsync(List<int> collectionReturnIds, CancellationToken cancellationToken)
        {
            var strCollectionReturnIds = string.Join("&collectionReturnIds=", collectionReturnIds).Substring(0);

            string url = $"{_baseUrl}/api/summarisation/return-totals/?collectionReturnIds={strCollectionReturnIds}";

            var data = await GetAsync<List<SummarisationTotal>>(url, cancellationToken);

            return data;
        }
    }
}