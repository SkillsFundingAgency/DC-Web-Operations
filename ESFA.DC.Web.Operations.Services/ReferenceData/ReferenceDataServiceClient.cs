using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Job;
using ESFA.DC.Web.Operations.Models.ReferenceData;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Http;

namespace ESFA.DC.Web.Operations.Services.ReferenceData
{
    public class ReferenceDataServiceClient : IReferenceDataServiceClient
    {
        private readonly string _baseUrl;
        private readonly IHttpClientService _httpClientService;

        public ReferenceDataServiceClient(ApiSettings apiSettings, IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<bool> IsReferenceDataCollectionExpired(string collectionName, CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}/api/returns-calendar/expired/{collectionName}";

            return await _httpClientService.GetAsync<bool>(url, cancellationToken);
        }

        public async Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmittedFilesPerCollectionAsync(string api, string collectionName, CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}{api}file-uploads/{collectionName}";

            return await _httpClientService.GetAsync<IEnumerable<FileUploadJobMetaDataModel>>(url, cancellationToken);
        }
    }
}
