using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.DashBoard
{
    public sealed class DashBoardService : BaseHttpClientService, IDashBoardService
    {
        private readonly string _baseUrl;

        public DashBoardService(ApiSettings apiSettings, IJsonSerializationService jsonSerializationService, HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<string> GetStatsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetDataAsync($"{_baseUrl}/api/dashboard/stats", cancellationToken);
        }
    }
}