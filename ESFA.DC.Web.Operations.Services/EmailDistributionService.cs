using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EmailDistribution.Models;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services
{
    public class EmailDistributionService : IEmailDistributionService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;

        public EmailDistributionService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = $"{apiSettings.JobManagementApiBaseUrl}/api/email-distribution";
        }

        public async Task<List<RecipientGroup>> GetEmailRecipientGroups(CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = $"{_baseUrl}/groups";

            return await NullCoalesceGetAsync(url, new List<RecipientGroup>(), cancellationToken);
        }

        public async Task<RecipientGroup> GetGroup(int recipientGroupId, CancellationToken cancellationToken = default)
        {
            var url = $"{_baseUrl}/groups/{recipientGroupId}";

            return await NullCoalesceGetAsync(url, new RecipientGroup(), cancellationToken);
        }

        public async Task<EmailTemplate> GetEmailTemplate(int emailId, CancellationToken cancellationToken = default)
        {
            var url = $"{_baseUrl}/templates/{emailId}";

            return await NullCoalesceGetAsync(url, new EmailTemplate(), cancellationToken);
        }

        public async Task<List<EmailTemplate>> GetEmailTemplates(CancellationToken cancellationToken = default)
        {
            var url = $"{_baseUrl}/templates";

            return await NullCoalesceGetAsync(url, new List<EmailTemplate>(), cancellationToken);
        }

        public async Task<List<Recipient>> GetGroupRecipients(int recipientGroupId, CancellationToken cancellationToken = default)
        {
            var url = $"{_baseUrl}/recipients/{recipientGroupId}";

            return await NullCoalesceGetAsync(url, new List<Recipient>(), cancellationToken);
        }

        public async Task<bool> IsDuplicateGroupName(string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await GetEmailRecipientGroups(cancellationToken);

            return result.Any(x => x.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> SaveEmailTemplate(EmailTemplate template, CancellationToken cancellationToken = default)
        {
            await _httpClientService.SendDataAsync(_baseUrl + "/templates", template, cancellationToken);

            return true;
        }

        public async Task<bool> RemoveGroup(int recipientGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await _httpClientService.SendDataAsync(_baseUrl + "/groups/remove", recipientGroupId, cancellationToken);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> SaveGroup(string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendDataAsync(_baseUrl + "/groups", groupName, cancellationToken);

            return true;
        }

        public async Task<HttpRawResponse> SaveRecipientAsync(Recipient recipient, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClientService.SendDataAsyncRawResponse(_baseUrl + "/recipients", recipient, cancellationToken);
            return response;
        }

        public async Task<bool> RemoveRecipient(int recipientId, int recipientGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendDataAsync($"{_baseUrl}/recipients/remove/{recipientId}/{recipientGroupId}", string.Empty, cancellationToken);
            return true;
        }

        private async Task<T> NullCoalesceGetAsync<T>(string url, T defaultValue, CancellationToken cancellationToken)
            where T : class
        {
            var result = await _httpClientService.GetAsync<T>(url, cancellationToken);

            return result ?? defaultValue;
        }
    }
}