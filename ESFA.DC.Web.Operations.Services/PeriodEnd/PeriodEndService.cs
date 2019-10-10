using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models.PeriodEnd;
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

        public async Task StartPeriodEnd(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendDataAsync($"{_baseUrl}/api/period-end/{year}/{period}/start", cancellationToken);
        }

        public async Task Proceed(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendDataAsync(_baseUrl + $"/api/period-end/{year}/{period}/{path}/proceed", cancellationToken);
        }

        public async Task ToggleReferenceDataJobs(bool pause, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendDataAsync(_baseUrl + $"/api/period-end/reference-data-jobs/{pause}", cancellationToken);
        }

        public async Task PublishProviderReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendDataAsync(_baseUrl + $"/api/period-end/provider-reports/{year}/{period}/publish", cancellationToken);
        }

        public async Task PublishMcaReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendDataAsync(_baseUrl + $"/api/period-end/mca-reports/{year}/{period}/publish", cancellationToken);
        }

        public async Task<string> GetPathItemStates(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string data = await GetDataAsync(_baseUrl + $"/api/period-end/states/{year}/{period}", cancellationToken);
            return data;
        }

        public async Task ClosePeriodEnd(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendDataAsync(_baseUrl + $"/api/period-end/{year}/{period}/close", cancellationToken);
        }

        public async Task<string> GetFailedJobs(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string data = await GetDataAsync(_baseUrl + $"/api/job/failedJobs/{year}/{period}", cancellationToken);

            return data;
        }

        public async Task<string> GetReferenceDataJobs(CancellationToken cancellationToken = default(CancellationToken))
        {
            string data = await GetDataAsync(_baseUrl + $"/api/period-end/reference-data-jobs", cancellationToken);

            return data;
        }

        public async Task ReSubmitFailedJob(long jobId)
        {
            var jobStatusDto = new JobStatusDto(jobId, Convert.ToInt32(JobStatusType.Ready));
            await SendDataAsync(_baseUrl + $"/api/job/{JobStatusType.Ready}", jobStatusDto);
        }

        public async Task<IEnumerable<ReportDetails>> GetPeriodEndReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<IEnumerable<ReportDetails>>(
                await GetDataAsync(_baseUrl + $"/api/period-end/reports/{year}/{period}", cancellationToken));

            return data;
        }

        public async Task<IEnumerable<ReportDetails>> GetMcaReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<IEnumerable<ReportDetails>>(
                await GetDataAsync(_baseUrl + $"/api/period-end/mca-reports/{year}/{period}", cancellationToken));

            return data;
        }

        public async Task<IEnumerable<CollectionStats>> GetCollectionStats(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<IEnumerable<CollectionStats>>(
                await GetDataAsync(_baseUrl + $"/api/period-end/collectionstats/{year}/{period}", cancellationToken));

            return data;
        }

        public async Task<IEnumerable<ReportDetails>> GetSampleReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<IEnumerable<ReportDetails>>(
                await GetDataAsync(_baseUrl + $"/api/period-end/reports/{year}/{period}/samples", cancellationToken));

            return data;
        }
     }
}