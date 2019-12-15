using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Settings.Models;
using MoreLinq;
using Organisation = ESFA.DC.CollectionsManagement.Models.Organisation;
using ProviderSearchResult = ESFA.DC.Web.Operations.Models.Provider.ProviderSearchResult;

namespace ESFA.DC.Web.Operations.Services.Provider
{
    public class AddNewProviderService : BaseHttpClientService, IAddNewProviderService
    {
        private readonly ILogger _logger;
        private readonly string _baseUrl;

        public AddNewProviderService(
                IJsonSerializationService jsonSerializationService,
                ILogger logger,
                ApiSettings apiSettings,
                HttpClient httpClient)
                : base(jsonSerializationService, httpClient)
        {
            _logger = logger;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<Models.Provider.Provider> GetProvider(long ukprn, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<ProviderDetail>(
                await GetDataAsync($"{_baseUrl}/api/org/{ukprn}", cancellationToken));

            return new Models.Provider.Provider(data.Name, data.Ukprn, data.Upin, null);
        }

        public async Task<HttpRawResponse> AddProvider(Models.Provider.Provider provider, CancellationToken cancellationToken = default)
        {
            var organisationDto = new Organisation() { Name = provider.Name, Ukprn = provider.Ukprn, IsMca = provider.IsMca.GetValueOrDefault() };
            return await SendDataAsyncRawResponse($"{_baseUrl}/api/org/add", organisationDto, cancellationToken);
        }

        public async Task<long> SubmitJob(Models.Job.Job submittedJob, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(submittedJob?.FileName))
            {
                throw new ArgumentException("submission message should have file name");
            }

            var job = new FileUploadJob()
            {
                Ukprn = submittedJob.Ukprn,
                Priority = 1,
                Status = JobStatusType.Ready,
                CreatedBy = submittedJob.SubmittedBy,
                FileName = submittedJob.FileName,
                IsFirstStage = true,
                StorageReference = submittedJob.StorageReference,
                FileSize = submittedJob.FileSizeBytes,
                CollectionName = submittedJob.CollectionName,
                PeriodNumber = submittedJob.Period,
                NotifyEmail = submittedJob.NotifyEmail,
                TermsAccepted = submittedJob.TermsAccepted,
            };

            var response = await SendDataAsync($"{_baseUrl}/job", job, cancellationToken);
            long.TryParse(response, out var result);

            return result;
        }
    }
}
