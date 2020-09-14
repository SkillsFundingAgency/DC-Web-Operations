using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services
{
    public class ApiAvailabilityService : BaseHttpClientService, IApiAvailabilityService
    {
        private readonly string _baseUrl;

        public ApiAvailabilityService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            IDateTimeProvider dateTimeProvider,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(routeFactory, jsonSerializationService, dateTimeProvider, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task SetApiAvailabilityAsync(string apiName, string apiUpdateProcess, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
        {
            var apiAvailability = new ApiAvailabilityDto { ApiName = apiName, Process = apiUpdateProcess, Enabled = enabled };
            await SendDataAsync($"{_baseUrl}/api/apiavailability/set", apiAvailability, cancellationToken);
        }
    }
}
