using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Settings.Models;
using Organisation = ESFA.DC.CollectionsManagement.Models.Organisation;

namespace ESFA.DC.Web.Operations.Services.Provider
{
    public class AddNewProviderService : BaseHttpClientService, IAddNewProviderService
    {
        private readonly string _baseUrl;

        public AddNewProviderService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(routeFactory, jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<Models.Provider.Provider> GetProvider(long ukprn, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<ProviderDetail>(
                await GetDataAsync($"{_baseUrl}/api/org/{ukprn}", cancellationToken));

            return new Models.Provider.Provider(data.Name, data.Ukprn, data.Upin, data.IsMCA);
        }

        public async Task<bool> AddProvider(Models.Provider.Provider provider, CancellationToken cancellationToken = default)
        {
            var organisationDto = new Organisation { Name = provider.Name, Ukprn = provider.Ukprn, IsMca = provider.IsMca.GetValueOrDefault() };
            var response = await SendDataAsyncRawResponse($"{_baseUrl}/api/org/add", organisationDto, cancellationToken);

            if (response.StatusCode == (int)HttpStatusCode.Conflict)
            {
                var updateResponse = await SendDataAsyncRawResponse($"{_baseUrl}/api/org/update", organisationDto, cancellationToken);
                if (updateResponse.StatusCode == (int)HttpStatusCode.OK)
                {
                    return true;
                }
            }
            else if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
