using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Models.Provider;
using ESFA.DC.Web.Operations.Settings.Models;
using MoreLinq;

namespace ESFA.DC.Web.Operations.Services.Provider
{
    public class SearchProviderService : BaseHttpClientService, ISearchProviderService
    {
        private readonly ILogger _logger;
        private readonly string _baseUrl;

        public SearchProviderService(
            IJsonSerializationService jsonSerializationService,
            ILogger logger,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _logger = logger;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<IEnumerable<ProviderSearchResult>> GetNewProviderSearchAsync(string query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<Jobs.Model.ProviderSearchResult>(
                await GetDataAsync($"{_baseUrl}/api/org/search/new/{query}", cancellationToken));

            var results = new List<ProviderSearchResult>();
            data.Providers.ForEach(p => results.Add(new ProviderSearchResult(p.Name, p.Ukprn, p.Upin, p.TradingName)));

            return results;
        }

        public async Task<IEnumerable<ProviderSearchResult>> GetExistingProviderSearchAsync(string query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<Jobs.Model.ProviderSearchResult>(
                await GetDataAsync($"{_baseUrl}/api/org/search/existing/{query}", cancellationToken));

            var results = new List<ProviderSearchResult>();
            data.Providers.ForEach(p => results.Add(new ProviderSearchResult(p.Name, p.Ukprn, p.Upin, p.TradingName)));

            return results;
        }
    }
}
