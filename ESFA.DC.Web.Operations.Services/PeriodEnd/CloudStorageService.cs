using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Models.ALLF;
using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class CloudStorageService : ICloudStorageService
    {
        private readonly ILogger _logger;
        private readonly ISerializationService _serializationService;
        private readonly AzureStorageSection _azureStorageConfig;

        private Dictionary<string, CloudBlobContainer> _containerReferences = new Dictionary<string, CloudBlobContainer>();

        private readonly BlobRequestOptions _requestOptions = new BlobRequestOptions
        {
            RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(5.0), 3),
            DisableContentMD5Validation = true,
            StoreBlobContentMD5 = false
        };

        public CloudStorageService(
            ILogger logger,
            ISerializationService serializationService,
            AzureStorageSection azureStorageConfig)
        {
            _logger = logger;
            _serializationService = serializationService;
            _azureStorageConfig = azureStorageConfig;

            ServicePointManager.DefaultConnectionLimit = 100;
            ThreadPool.SetMinThreads(100, 100);
        }

        public CloudBlobContainer GetStorageContainer(string containerName)
        {
            CloudBlobContainer containerReference;

            if (_containerReferences.TryGetValue(containerName, out containerReference))
            {
                return containerReference;
            }

            try
            {
                _containerReferences.Add(containerName, CloudStorageAccount.Parse(_azureStorageConfig.ConnectionString).CreateCloudBlobClient().GetContainerReference(containerName));

                return _containerReferences[containerName];
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to open blob container: {containerName}", e);
                throw;
            }
        }

        public async Task<SubmissionSummary> GetSubmissionSummary(CloudBlobContainer container, string fileName, CancellationToken cancellationToken)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            try
            {
                var blobReference = container.GetBlockBlobReference(fileName);

                if (await blobReference.ExistsAsync(cancellationToken))
                {
                    using (var stream = await blobReference
                        .OpenReadAsync(null, _requestOptions, null, cancellationToken))
                    {
                        return _serializationService.Deserialize<SubmissionSummary>(stream);
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to read blob: {fileName} Container:{container.Name} ContainerUri:{container.Uri}", e);
                throw;
            }
        }
    }
}