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
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.NCS
{
    public class NCSPeriodEndService : BaseHttpClientService, INCSPeriodEndService
    {
        private const string CollectionType = CollectionTypes.NCS;
        private const string api = "/api/period-end-ncs/";
        private readonly string _baseUrl;

        public NCSPeriodEndService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            IDateTimeProvider dateTimeProvider,
            HttpClient httpClient)
            : base(routeFactory, jsonSerializationService, dateTimeProvider, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task InitialisePeriodEndAsync(int year, int period, CancellationToken cancellationToken)
        {
            await SendAsync($"{_baseUrl}{api}{year}/{period}/{CollectionType}/initialise", cancellationToken);
        }

        public async Task StartPeriodEndAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync($"{_baseUrl}{api}{year}/{period}/{CollectionType}/start", cancellationToken);
        }

        public async Task CollectionClosedEmailSentAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync($"{_baseUrl}{api}{year}/{period}/{CollectionType}/collection-closed", cancellationToken);
        }

        public async Task ProceedAsync(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync(_baseUrl + $"{api}{year}/{period}/{path}/proceed", cancellationToken);
        }

        public async Task<string> GetPrepStateAsync(int? year, int? period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string data = await GetDataAsync(_baseUrl + $"{api}states-prep/{CollectionType}/{year}/{period}", cancellationToken);
            return data;
        }

        public async Task<string> GetPathItemStatesAsync(int? year, int? period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string data = await GetDataAsync(_baseUrl + $"{api}states-main/{CollectionType}/{year}/{period}", cancellationToken);
            return data;
        }

        public async Task ClosePeriodEndAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync(_baseUrl + $"{api}{year}/{period}/{CollectionType}/close", cancellationToken);
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