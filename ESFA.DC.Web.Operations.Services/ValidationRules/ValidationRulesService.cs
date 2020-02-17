using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.ValidationRules;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

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

        public async Task<long> GenerateReport(string rule, int year, string createdBy, CancellationToken cancellationToken = default(CancellationToken))
        {
            string collectionName = Constants.ValidationRuleDetailsReportCollectionName;
            ValidationRuleDetailsReportJob job = new ValidationRuleDetailsReportJob()
            {
                CollectionName = collectionName,
                Status = Jobs.Model.Enums.JobStatusType.Ready,
                JobId = 0,
                SelectedCollectionYear = year,
                Rule = rule,
                CreatedBy = createdBy
            };

            string url = $"{_baseUrl}/api/job/validationruledetailsreport";
            var json = _jsonSerializationService.Serialize(job);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            long.TryParse(result, out var jobId);

            return jobId;
        }
    }
}
