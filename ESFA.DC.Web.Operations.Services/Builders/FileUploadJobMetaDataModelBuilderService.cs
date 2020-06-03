using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.ALLF;
using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;

namespace ESFA.DC.Web.Operations.Services.Builders
{
    public class FileUploadJobMetaDataModelBuilderService : IFileUploadJobMetaDataModelBuilderService
    {
        private readonly AzureStorageSection _azureStorageConfig;
        private readonly ISerializationService _serializationService;

        private readonly BlobRequestOptions _requestOptions = new BlobRequestOptions
        {
            RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(5.0), 3),
            DisableContentMD5Validation = true,
            StoreBlobContentMD5 = false
        };

        public FileUploadJobMetaDataModelBuilderService(
            AzureStorageSection azureStorageConfig,
            ISerializationService serializationService)
        {
            _azureStorageConfig = azureStorageConfig;
            _serializationService = serializationService;
        }

        public async Task<FileUploadJobMetaDataModel> PopulateFileUploadJobMetaDataModel(
            FileUploadJobMetaDataModel file,
            string reportName,
            string resultsReportName,
            CloudBlobContainer container,
            string periodPrefix,
            CancellationToken cancellationToken)
        {
            file.ReportName = $"{reportName} {file.FileName}";

            var resultFileName = $"{periodPrefix}{file.PeriodNumber}/{file.JobId}/{resultsReportName} {Path.GetFileNameWithoutExtension(file.FileName)}.json";

            SubmissionSummary result;
            using (var stream = await container
                .GetBlockBlobReference(resultFileName)
                .OpenReadAsync(null, _requestOptions, null, cancellationToken))
            {
                result = _serializationService.Deserialize<SubmissionSummary>(stream);
            }

            if (result == null)
            {
                return file;
            }

            file.RecordCount = result.RecordCount;
            file.ErrorCount = result.ErrorCount;

            return file;
        }
    }
}