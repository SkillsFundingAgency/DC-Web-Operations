using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models.Summarisation;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class PeriodEndService : BaseHttpClientService, IPeriodEndService
    {
        private readonly string _baseUrl;

        public PeriodEndService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            IDateTimeProvider dateTimeProvider,
            HttpClient httpClient)
            : base(routeFactory, jsonSerializationService, dateTimeProvider, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task InitialisePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken)
        {
            await SendAsync($"{_baseUrl}/api/period-end/{year}/{period}/{collectionType}/initialise", cancellationToken);
        }

        public async Task StartPeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync($"{_baseUrl}/api/period-end/{year}/{period}/{collectionType}/start", cancellationToken);
        }

        public async Task CollectionClosedEmailSentAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync($"{_baseUrl}/api/period-end/{year}/{period}/{collectionType}/collection-closed", cancellationToken);
        }

        public async Task ProceedAsync(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync(_baseUrl + $"/api/period-end/{year}/{period}/{path}/proceed", cancellationToken);
        }

        public async Task ToggleReferenceDataJobsAsync(int year, int period, bool pause, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync(_baseUrl + $"/api/period-end/reference-data-jobs/{year}/{period}/{pause}", cancellationToken);
        }

        public async Task PublishProviderReportsAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync(_baseUrl + $"/api/period-end/provider-reports/{year}/{period}/{collectionType}/publish", cancellationToken);
        }

        public async Task PublishMcaReportsAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync(_baseUrl + $"/api/period-end/mca-reports/{year}/{period}/{collectionType}/publish", cancellationToken);
        }

        public async Task<string> GetPrepStateAsync(int? year, int? period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            string data = await GetDataAsync(_baseUrl + $"/api/period-end/states-prep/{collectionType}/{year}/{period}", cancellationToken);
            return data;
        }

        public async Task<string> GetPathItemStatesAsync(int? year, int? period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            string data = await GetDataAsync(_baseUrl + $"/api/period-end/states-main/{collectionType}/{year}/{period}", cancellationToken);
            return data;
        }

        public async Task<bool> ClosePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await SendAsync(_baseUrl + $"/api/period-end/{year}/{period}/{collectionType}/close", cancellationToken);
            return Convert.ToBoolean(result);
        }

        public async Task ReSubmitFailedJobAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var rawjob = await GetDataAsync(_baseUrl + $"/api/job/{jobId}", cancellationToken);
            var oldJob = _jsonSerializationService.Deserialize<FileUploadJob>(rawjob);

            if (oldJob.CollectionName.Contains(Constants.DASSubmission))
            {
                // For DAS jobs, issue a clone command
                await SendDataAsync($"{_baseUrl}/api/job/clone-job", jobId, cancellationToken);
            }
            else
            {
                // For all others, reset the status back to 1
                var jobStatusDto = new JobStatusDto(jobId, Convert.ToInt32(JobStatusType.Ready));
                await SendDataAsync($"{_baseUrl}/api/job/{JobStatusType.Ready}", jobStatusDto, cancellationToken);
            }
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

        public async Task<List<SummarisationCollectionReturnCode>> GetSummarisationCollectionCodesAsync(string collectionType, int year, int period, CancellationToken cancellationToken)
        {
            var path = $"/api/summarisation/return-codes-for-period";
            var segments = new List<string>
            {
                path,
                collectionType,
                year.ToString(),
                period.ToString()
            };

            var data = await GetAsync<List<SummarisationCollectionReturnCode>>(_baseUrl, segments);

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