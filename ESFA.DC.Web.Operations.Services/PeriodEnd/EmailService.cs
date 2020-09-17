using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class EmailService : IEmailService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;

        public EmailService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = $"{apiSettings.JobManagementApiBaseUrl}/api/period-end-email";
        }

        public async Task SendEmail(int hubEmailId, int periodNumber, string periodPrefix, CancellationToken cancellationToken = default)
        {
            var url = $"{_baseUrl}/{hubEmailId}/{periodNumber}/{periodPrefix}";
            await _httpClientService.SendAsync(url, cancellationToken);
        }
    }
}