using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.DashBoard
{
    public sealed class DashBoardService : IDashBoardService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;

        public DashBoardService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<string> GetStatsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _httpClientService.GetDataAsync($"{_baseUrl}/api/dashboard/stats", cancellationToken);
        }
    }
}