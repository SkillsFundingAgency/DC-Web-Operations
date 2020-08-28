using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class EmailService : BaseHttpClientService, IEmailService
    {
        private readonly string _baseUrl;

        public EmailService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            HttpClient httpClient,
            IDateTimeProvider dateTimeProvider,
            ApiSettings apiSettings)
            : base(routeFactory, jsonSerializationService, dateTimeProvider, httpClient)
        {
            _baseUrl = $"{apiSettings.JobManagementApiBaseUrl}/api/period-end-email";
        }

        public async Task SendEmail(int hubEmailId, int periodNumber, string periodPrefix, CancellationToken cancellationToken = default)
        {
            await SendAsync(_baseUrl + $"/{hubEmailId}/{periodNumber}/{periodPrefix}", cancellationToken);
        }
    }
}