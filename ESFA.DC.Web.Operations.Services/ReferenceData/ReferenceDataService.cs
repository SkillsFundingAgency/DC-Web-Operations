using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.Job;
using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.AspNetCore.Http;

namespace ESFA.DC.Web.Operations.Services.ReferenceData
{
    public class ReferenceDataService : IReferenceDataService
    {
        private readonly ICollectionsService _collectionsService;
        private readonly IJobService _jobService;
        private readonly IStorageService _storageService;
        private readonly AzureStorageSection _azureStorageConfig;
        private readonly ILogger _logger;

        public ReferenceDataService(
            ICollectionsService collectionsService,
            IJobService jobService,
            IStorageService storageService,
            AzureStorageSection azureStorageConfig,
            ILogger logger)
        {
            _collectionsService = collectionsService;
            _jobService = jobService;
            _storageService = storageService;
            _azureStorageConfig = azureStorageConfig;
            _logger = logger;
        }

        public async Task SubmitJob(int period, string collectionName, string userName, string email, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                return;
            }

            var collection = await _collectionsService.GetCollectionAsync(collectionName, cancellationToken);

            try
            {
                var fileName = Path.GetFileName(file.FileName);

                var job = new JobSubmission
                {
                    CollectionName = collection.CollectionTitle,
                    FileName = fileName,
                    FileSizeBytes = file.Length,
                    SubmittedBy = userName,
                    NotifyEmail = email,
                    StorageReference = collection.StorageReference,
                    Period = period,
                    CollectionYear = collection.CollectionYear
                };

                // add to the queue
                await _jobService.SubmitJob(job, cancellationToken);

                await (await _storageService.GetAzureStorageReferenceService(_azureStorageConfig.ConnectionString, collection.StorageReference))
                    .SaveAsync(fileName, file.OpenReadStream(), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error trying to submit Campus Identifiers Reference Data file with name : {file.Name}", ex);
                throw;
            }
        }
    }
}
