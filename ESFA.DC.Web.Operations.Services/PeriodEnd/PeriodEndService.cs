using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models.Summarisation;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class PeriodEndService : IPeriodEndService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;

        public PeriodEndService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task InitialisePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken)
        {
            await _httpClientService.SendAsync($"{_baseUrl}/api/period-end/{year}/{period}/{collectionType}/initialise", cancellationToken);
        }

        public async Task StartPeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendAsync($"{_baseUrl}/api/period-end/{year}/{period}/{collectionType}/start", cancellationToken);
        }

        public async Task CollectionClosedEmailSentAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendAsync($"{_baseUrl}/api/period-end/{year}/{period}/collection-closed", cancellationToken);
        }

        public async Task ProceedAsync(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendAsync($"{_baseUrl}/api/period-end/{year}/{period}/{path}/proceed", cancellationToken);
        }

        public async Task ToggleReferenceDataJobsAsync(int year, int period, bool pause, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendAsync($"{_baseUrl}/api/period-end/reference-data-jobs/{year}/{period}/{pause}", cancellationToken);
        }

        public async Task PublishProviderReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendAsync($"{_baseUrl}/api/period-end/provider-reports/{year}/{period}/publish", cancellationToken);
        }

        public async Task PublishMcaReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendAsync($"{_baseUrl}/api/period-end/mca-reports/{year}/{period}/publish", cancellationToken);
        }

        public async Task<string> GetPrepStateAsync(int? year, int? period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _httpClientService.GetDataAsync($"{_baseUrl}/api/period-end/states-prep/{collectionType}/{year}/{period}", cancellationToken);
        }

        public async Task<string> GetPathItemStatesAsync(int? year, int? period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _httpClientService.GetDataAsync($"{_baseUrl}/api/period-end/states-main/{collectionType}/{year}/{period}", cancellationToken);
        }

        public async Task<bool> ClosePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _httpClientService.SendAsync($"{_baseUrl}/api/period-end/{year}/{period}/{collectionType}/close", cancellationToken);
            return Convert.ToBoolean(result);
        }

        public async Task ReSubmitFailedJobAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var oldJob = await _httpClientService.GetAsync<FileUploadJob>($"{_baseUrl}/api/job/{jobId}", cancellationToken);

            if (oldJob.CollectionName.Contains(Constants.DASSubmission))
            {
                // For DAS jobs, issue a clone command
                await _httpClientService.SendDataAsync($"{_baseUrl}/api/job/clone-job", jobId, cancellationToken);
            }
            else
            {
                // For all others, reset the status back to 1
                var jobStatusDto = new JobStatusDto(jobId, Convert.ToInt32(JobStatusType.Ready));
                await _httpClientService.SendDataAsync($"{_baseUrl}/api/job/{JobStatusType.Ready}", jobStatusDto, cancellationToken);
            }
        }

        public async Task<IEnumerable<ReportDetails>> GetPeriodEndReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_baseUrl}/api/period-end/reports/{year}/{period}";

            return await _httpClientService.GetAsync<IEnumerable<ReportDetails>>(url, cancellationToken);
        }

        public async Task<IEnumerable<ReportDetails>> GetMcaReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_baseUrl}/api/period-end/mca-reports/{year}/{period}";

            return await _httpClientService.GetAsync<IEnumerable<ReportDetails>>(url, cancellationToken);
        }

        public async Task<IEnumerable<CollectionStats>> GetCollectionStatsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_baseUrl}/api/period-end/collectionstats/{year}/{period}";

            return await _httpClientService.GetAsync<IEnumerable<CollectionStats>>(url, cancellationToken);
        }

        public async Task<IEnumerable<ReportDetails>> GetSampleReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_baseUrl}/api/period-end/reports/{year}/{period}/samples";

            return await _httpClientService.GetAsync<IEnumerable<ReportDetails>>(url, cancellationToken);
        }

        public async Task<List<SummarisationCollectionReturnCode>> GetLatestSummarisationCollectionCodesAsync(string collectionType, int numberOfPeriods, CancellationToken cancellationToken)
        {
            string url = $"{_baseUrl}/api/summarisation/return-codes/{collectionType}/{numberOfPeriods}";

            return await _httpClientService.GetAsync<List<SummarisationCollectionReturnCode>>(url, cancellationToken);
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

            return await _httpClientService.GetAsync<List<SummarisationCollectionReturnCode>>(_baseUrl, segments, cancellationToken);
        }

        public async Task<List<SummarisationTotal>> GetSummarisationTotalsAsync(List<int> collectionReturnIds, CancellationToken cancellationToken)
        {
            var strCollectionReturnIds = string.Join("&collectionReturnIds=", collectionReturnIds).Substring(0);

            string url = $"{_baseUrl}/api/summarisation/return-totals/?collectionReturnIds={strCollectionReturnIds}";

            var data = await _httpClientService.GetAsync<List<SummarisationTotal>>(url, cancellationToken);

            return data;
        }
    }
}