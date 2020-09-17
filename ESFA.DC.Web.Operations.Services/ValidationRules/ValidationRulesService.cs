using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.ValidationRules;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.ValidationRules
{
    public class ValidationRulesService : IValidationRulesService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;

        public ValidationRulesService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<IEnumerable<ValidationRule>> GetValidationRules(int year, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _httpClientService.GetAsync<IEnumerable<ValidationRule>>($"{_baseUrl}/api/validationrules/{year}", cancellationToken);
        }

        public async Task<long> GenerateReport(string rule, int year, string createdBy, CancellationToken cancellationToken = default(CancellationToken))
        {
            string collectionName = CollectionNames.ValidationRuleDetailsReportCollectionName;
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

            var result = await _httpClientService.SendDataAsync(url, job, cancellationToken);
            long.TryParse(result, out var jobId);

            return jobId;
        }
    }
}
