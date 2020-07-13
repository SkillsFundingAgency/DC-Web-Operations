﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class ValidityPeriodService : BaseHttpClientService, IValidityPeriodService
    {
        private readonly string _baseUrl;

        public ValidityPeriodService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(routeFactory, jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<string> GetValidityPeriodList(int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetDataAsync($"{_baseUrl}/api/validityperiod/validityperiodlist/{period}", cancellationToken);
        }

        public async Task<string> UpdateValidityPeriod(int period, object validityPeriods, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await PutDataAsync($"{_baseUrl}/api/validityperiod/updatevalidityperiod/{period}", validityPeriods, cancellationToken);
        }
    }
}