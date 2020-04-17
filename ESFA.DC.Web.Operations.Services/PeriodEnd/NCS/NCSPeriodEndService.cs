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
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.NCS
{
    public class NCSPeriodEndService : BaseHttpClientService, INCSPeriodEndService
    {
        private readonly string _baseUrl;
        private const string api = "/api/period-end-ncs/";

        public NCSPeriodEndService(
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task InitialisePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken)
        {
            await SendAsync($"{_baseUrl}{api}{year}/{period}/{collectionType}/initialise", cancellationToken);
        }

        public async Task StartPeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync($"{_baseUrl}{api}{year}/{period}/{collectionType}/start", cancellationToken);
        }

        public async Task CollectionClosedEmailSentAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync($"{_baseUrl}{api}{year}/{period}/collection-closed", cancellationToken);
        }

        public async Task ProceedAsync(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync(_baseUrl + $"{api}{year}/{period}/{path}/proceed", cancellationToken);
        }

        public async Task<string> GetPrepStateAsync(int? year, int? period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            string data = await GetDataAsync(_baseUrl + $"{api}states-prep/{collectionType}/{year}/{period}", cancellationToken);
            return data;
        }

        public async Task<string> GetPathItemStatesAsync(int? year, int? period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            string data = await GetDataAsync(_baseUrl + $"{api}states-main/{collectionType}/{year}/{period}", cancellationToken);
            return data;
        }

        public async Task ClosePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync(_baseUrl + $"{api}{year}/{period}/{collectionType}/close", cancellationToken);
        }

        public async Task ReSubmitFailedJobAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var jobStatusDto = new JobStatusDto(jobId, Convert.ToInt32(JobStatusType.Ready));
            await SendDataAsync($"{_baseUrl}/api/job/{JobStatusType.Ready}", jobStatusDto, cancellationToken);
        }

        public async Task<IEnumerable<ReportDetails>> GetPeriodEndReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_baseUrl}{api}reports/{year}/{period}";

            var data = await GetAsync<IEnumerable<ReportDetails>>(url, cancellationToken);

            return data;
        }
    }
}