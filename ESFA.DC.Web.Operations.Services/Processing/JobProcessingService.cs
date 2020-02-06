using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Models.JobsProcessing;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.Processing
{
    public class JobProcessingService : BaseHttpClientService, IJobProcessingService
    {
        private readonly string _baseUrl;

        public JobProcessingService(ApiSettings apiSettings, IJsonSerializationService jsonSerializationService, HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<JobsProcessingModel> GetJobsThatAreProcessing(CancellationToken cancellationToken = default)
        {
            var data = await GetDataAsync($"{_baseUrl}/api/job/jobsthatareprocessing", cancellationToken);
            return _jsonSerializationService.Deserialize<JobsProcessingModel>(data);
        }
    }
}
