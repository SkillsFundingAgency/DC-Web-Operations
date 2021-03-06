﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.NCS
{
    public class NCSPeriodEndService : INCSPeriodEndService
    {
        private const string CollectionType = CollectionTypes.NCS;
        private const string api = "/api/period-end-ncs/";
        private readonly string _baseUrl;
        private readonly IHttpClientService _httpClientService;

        public NCSPeriodEndService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task InitialisePeriodEndAsync(int year, int period, CancellationToken cancellationToken)
        {
            await _httpClientService.SendAsync($"{_baseUrl}{api}{year}/{period}/{CollectionType}/initialise", cancellationToken);
        }

        public async Task StartPeriodEndAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendAsync($"{_baseUrl}{api}{year}/{period}/{CollectionType}/start", cancellationToken);
        }

        public async Task CollectionClosedEmailSentAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendAsync($"{_baseUrl}{api}{year}/{period}/{CollectionType}/collection-closed", cancellationToken);
        }

        public async Task ProceedAsync(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendAsync($"{_baseUrl}{api}{year}/{period}/{path}/proceed", cancellationToken);
        }

        public async Task<string> GetPrepStateAsync(int? year, int? period, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _httpClientService.GetDataAsync($"{_baseUrl}{api}states-prep/{CollectionType}/{year}/{period}", cancellationToken);
        }

        public async Task<string> GetPathItemStatesAsync(int? year, int? period, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _httpClientService.GetDataAsync($"{_baseUrl}{api}states-main/{CollectionType}/{year}/{period}", cancellationToken);
        }

        public async Task ClosePeriodEndAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendAsync($"{_baseUrl}{api}{year}/{period}/{CollectionType}/close", cancellationToken);
        }

        public async Task ReSubmitFailedJobAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var jobStatusDto = new JobStatusDto(jobId, Convert.ToInt32(JobStatusType.Ready));
            await _httpClientService.SendDataAsync($"{_baseUrl}/api/job/{JobStatusType.Ready}", jobStatusDto, cancellationToken);
        }

        public async Task<IEnumerable<ReportDetails>> GetPeriodEndReportsAsync(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_baseUrl}{api}reports/{year}/{period}";

            return await _httpClientService.GetAsync<IEnumerable<ReportDetails>>(url, cancellationToken);
        }
    }
}