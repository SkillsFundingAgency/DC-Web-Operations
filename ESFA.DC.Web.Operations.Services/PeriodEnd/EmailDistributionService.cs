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

        public async Task<RecipientGroup> GetGroup(int recipientGroupId, CancellationToken cancellationToken = default)
        {
            var result = new RecipientGroup();
            string data = await GetDataAsync($"{_baseUrl}/groups/{recipientGroupId}", cancellationToken);
            if (!string.IsNullOrEmpty(data))
            {
                result = _jsonSerializationService.Deserialize<RecipientGroup>(data);
            }

            return result;
        }

        public async Task<EmailTemplate> GetEmailTemplate(int emailId, CancellationToken cancellationToken = default)
        {
            var result = new EmailTemplate();
            string data = await GetDataAsync($"{_baseUrl}/templates/{emailId}", cancellationToken);
            if (!string.IsNullOrEmpty(data))
            {
                result = _jsonSerializationService.Deserialize<EmailTemplate>(data);
            }

            return result;
        }

        public async Task<List<EmailTemplate>> GetEmailTemplates(CancellationToken cancellationToken = default)
        {
            var result = new List<EmailTemplate>();
            string data = await GetDataAsync(_baseUrl + "/templates", cancellationToken);
            if (!string.IsNullOrEmpty(data))
            {
                result = _jsonSerializationService.Deserialize<List<EmailTemplate>>(data);
            }

            return result;
        }

        public async Task<List<Recipient>> GetGroupRecipients(int recipientGroupId, CancellationToken cancellationToken = default)
        {
            var result = new List<Recipient>();
            string data = await GetDataAsync($"{_baseUrl}/recipients/{recipientGroupId}", cancellationToken);
            if (!string.IsNullOrEmpty(data))
            {
                result = _jsonSerializationService.Deserialize<List<Recipient>>(data);
            }

            return result;
        }

        public async Task<bool> IsDuplicateGroupName(string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await GetEmailRecipientGroups(cancellationToken);

            return result.Any(x => x.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> SaveEmailTemplate(EmailTemplate template, CancellationToken cancellationToken = default)
        {
            await SendDataAsync(_baseUrl + "/templates", template);

            return true;
        }

        public async Task<bool> RemoveGroup(int recipientGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendDataAsync(_baseUrl + "/groups/remove", recipientGroupId);

            return true;
        }

        public async Task<bool> SaveGroup(string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendDataAsync(_baseUrl + "/groups", groupName);

            return true;
        }

        public async Task<bool> SaveRecipient(Recipient recipient, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendDataAsync(_baseUrl + "/recipients", recipient);

            return true;
        }

        public async Task<bool> RemoveRecipient(int recipientId, int recipientGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendDataAsync($"{_baseUrl}/recipients/remove/{recipientId}/{recipientGroupId}", string.Empty);
            return true;
        }
    }
}