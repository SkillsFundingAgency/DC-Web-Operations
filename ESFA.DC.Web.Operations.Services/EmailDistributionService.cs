using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EmailDistribution.Models;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services
{
    public class EmailDistributionService : BaseHttpClientService, IEmailDistributionService
    {
        private readonly string _baseUrl;

        public EmailDistributionService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            IDateTimeProvider dateTimeProvider,
            HttpClient httpClient,
            ApiSettings apiSettings)
            : base(routeFactory, jsonSerializationService, dateTimeProvider, httpClient)
        {
            _baseUrl = $"{apiSettings.JobManagementApiBaseUrl}/api/email-distribution";
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
            await SendDataAsync(_baseUrl + "/templates", template, cancellationToken);

            return true;
        }

        public async Task<bool> RemoveGroup(int recipientGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await SendDataAsync(_baseUrl + "/groups/remove", recipientGroupId, cancellationToken);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> SaveGroup(string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendDataAsync(_baseUrl + "/groups", groupName, cancellationToken);

            return true;
        }

        public async Task<HttpRawResponse> SaveRecipientAsync(Recipient recipient, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await SendDataAsyncRawResponse(_baseUrl + "/recipients", recipient, cancellationToken);
            return response;
        }

        public async Task<bool> RemoveRecipient(int recipientId, int recipientGroupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendDataAsync($"{_baseUrl}/recipients/remove/{recipientId}/{recipientGroupId}", string.Empty, cancellationToken);
            return true;
        }
    }
}