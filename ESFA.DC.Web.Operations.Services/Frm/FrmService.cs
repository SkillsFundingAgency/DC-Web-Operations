﻿using System;
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
        private readonly string _jobApiUrl;

        public FrmService(
            IFileService fileService,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _jobApiUrl = $"{apiSettings.JobManagementApiBaseUrl}/api/job";
            _httpClient = httpClient;
        }

        public async Task<int> GetFrmStatus(long? jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!jobId.HasValue)
            {
                throw new Exception("Missing 'jobId' parameter");
            }

            string url = $"{_jobApiUrl}/{jobId}/status";
            var response = await GetDataAsync(url, cancellationToken);
            int.TryParse(response, out var result);
            return result;
        }

        public async Task<long> RunValidation(string containerName, string folderKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            string collectionName = Constants.FrmReportCollectionName;
            FrmReportsJob job = new FrmReportsJob()
            {
                CollectionName = collectionName,
                Status = Jobs.Model.Enums.JobStatusType.Ready,
                JobId = 0,
                SourceContainerName = containerName,
                SourceFolderKey = folderKey
            };

            string url = $"{_jobApiUrl}/frm/validate";
            var json = _jsonSerializationService.Serialize(job);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            long.TryParse(result, out var jobId);

            return jobId;
        }

        public async Task<long> RunPublish(long jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            string collectionName = Constants.FrmReportCollectionName;

            JobStatusDto statusDto = new JobStatusDto()
            {
                JobId = jobId,
                JobStatus = 1
            };

            string url = $"{_jobApiUrl}/1";
            var json = _jsonSerializationService.Serialize(statusDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            return jobId;
        }
    }
}