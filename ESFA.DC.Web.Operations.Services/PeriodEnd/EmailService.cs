﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class EmailService : BaseHttpClientService, IEmailService
    {
        private readonly string _baseUrl;

        public EmailService(
            IJsonSerializationService jsonSerializationService,
            HttpClient httpClient,
            ApiSettings apiSettings)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = $"{apiSettings.JobManagementApiBaseUrl}/api/period-end-email";
        }

        public async Task SendEmail(int hubEmailId, int periodNumber, CancellationToken cancellationToken = default)
        {
            await SendDataAsync(_baseUrl + $"/{hubEmailId}/{periodNumber}", cancellationToken);
        }
    }
}