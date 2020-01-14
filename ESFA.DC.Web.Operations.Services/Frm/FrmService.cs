using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Frm;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Frm
{
    public class FrmService : BaseHttpClientService, IFrmService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public FrmService(
            IFileService fileService,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
            _httpClient = httpClient;
        }

        public async Task<int> GetFrmStatus(long? jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!jobId.HasValue)
            {
                throw new Exception("Missing 'jobId' parameter");
            }

            string url = $"{_baseUrl}/api/job/{jobId}/status";
            var response = await GetDataAsync(url, cancellationToken);
            int.TryParse(response, out var result);
            return result;
        }

        public async Task<long> RunFrm(string stage, int collectionYear, int collectionPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            long jobId = -1;

            string collectionName = Constants.FrmReportCollectionName;
            FileUploadJob job = new FileUploadJob()
            {
                CollectionYear = collectionYear,
                PeriodNumber = collectionPeriod,
                CollectionName = collectionName,
                StorageReference = string.Format(Constants.FrmContainerName, collectionYear),
                Status = Jobs.Model.Enums.JobStatusType.Ready,
                JobId = 0
            };

            string url = $"{_baseUrl}/api/job";
            var json = _jsonSerializationService.Serialize(job);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            long.TryParse(result, out jobId);

            return jobId;
        }
    }
}