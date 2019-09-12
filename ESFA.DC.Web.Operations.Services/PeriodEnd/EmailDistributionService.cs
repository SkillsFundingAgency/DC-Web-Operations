using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models.EmailDistribution;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class EmailDistributionService : BaseHttpClientService, IEmailDistributionService
    {
        private readonly string _baseUrl;

        public EmailDistributionService(
            IJsonSerializationService jsonSerializationService,
            HttpClient httpClient,
            ApiSettings apiSettings)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = $"{apiSettings.JobManagementApiBaseUrl}api/period-end/email-distribution";
        }

        public async Task<List<RecipientGroup>> GetEmailRecipientGroups(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<RecipientGroup>();
            string data = await GetDataAsync(_baseUrl + "/groups", cancellationToken);
            if (!string.IsNullOrEmpty(data))
            {
               result = _jsonSerializationService.Deserialize<List<RecipientGroup>>(data);
            }

            return result.Where(x => x.Enabled).ToList();
        }
    }
}