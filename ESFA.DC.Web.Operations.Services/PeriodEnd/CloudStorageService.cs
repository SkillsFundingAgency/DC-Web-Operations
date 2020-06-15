using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models.ALLF;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class CloudStorageService : ICloudStorageService
    {
        private readonly ISerializationService _serializationService;
        private readonly AzureStorageSection _azureStorageConfig;

        private readonly BlobRequestOptions _requestOptions = new BlobRequestOptions
        {
            RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(5.0), 3),
            DisableContentMD5Validation = true,
            StoreBlobContentMD5 = false
        };

        public CloudStorageService(
            ISerializationService serializationService,
            AzureStorageSection azureStorageConfig)
        {
            _serializationService = serializationService;
            _azureStorageConfig = azureStorageConfig;
        }

        public CloudBlobContainer GetStorageContainer()
        {
            return CloudStorageAccount.Parse(_azureStorageConfig.ConnectionString).CreateCloudBlobClient().GetContainerReference(Constants.ALLFStorageContainerName);
        }

        public CloudBlobContainer GetReferenceDataStorageContainer()
        {
            return CloudStorageAccount.Parse(_azureStorageConfig.ConnectionString).CreateCloudBlobClient().GetContainerReference(Constants.ReferenceDataStorageContainerName);
        }

        public async Task<SubmissionSummary> GetSubmissionSummary(CloudBlobContainer container, string fileName, CancellationToken cancellationToken)
        {
            using (var stream = await container
                .GetBlockBlobReference(fileName)
                .OpenReadAsync(null, _requestOptions, null, cancellationToken))
            {
                return _serializationService.Deserialize<SubmissionSummary>(stream);
            }
        }
    }
}