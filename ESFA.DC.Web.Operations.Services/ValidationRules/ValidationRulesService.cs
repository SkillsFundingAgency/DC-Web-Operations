using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.ValidationRules;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.ValidationRules
{
    public class ValidationRulesService : BaseHttpClientService, IValidationRulesService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ValidationRulesService(
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<string>> GetValidationRules(int year, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<IEnumerable<string>>(await GetDataAsync($"{_baseUrl}/api/validationrules/{year}", cancellationToken));
            return data;
        }
    }
}
