using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EmailDistribution.Models;
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
            _baseUrl = $"{apiSettings.JobManagementApiBaseUrl}api/email-distribution";
        }

        public async Task<List<RecipientGroup>> GetEmailRecipientGroups(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<RecipientGroup>();
            string data = await GetDataAsync(_baseUrl + "/groups", cancellationToken);
            if (!string.IsNullOrEmpty(data))
            {
               result = _jsonSerializationService.Deserialize<List<RecipientGroup>>(data);
            }

            return result;
        }

        public async Task<bool> IsDuplicateGroupName(string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<RecipientGroup>();
            string data = await GetDataAsync(_baseUrl + "/groups", cancellationToken);
            if (!string.IsNullOrEmpty(data))
            {
                result = _jsonSerializationService.Deserialize<List<RecipientGroup>>(data);
            }

            return result.Any(x => x.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> SaveGroup(string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<RecipientGroup>();
            await SendDataAsync(_baseUrl + "/groups", groupName);

            return true;
        }

        public async Task<bool> SaveRecipient(Recipient recipient, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<RecipientGroup>();
            await SendDataAsync(_baseUrl + "/recipients", recipient);

            return true;
        }
    }
}