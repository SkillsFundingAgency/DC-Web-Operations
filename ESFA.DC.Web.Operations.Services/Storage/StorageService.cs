using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.StaticFiles;

namespace ESFA.DC.Web.Operations.Services.Storage
{
    public class StorageService : IStorageService
    {
        private readonly IFileService _fileService;

        public StorageService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<Stream> GetFile(int collectionYear, string fileName, CancellationToken cancellationToken)
        {
            var containerName = Constants.PeriodEndBlobContainerName.Replace(Constants.CollectionYearToken, collectionYear.ToString());

            if (!await _fileService.ExistsAsync(fileName, containerName, cancellationToken))
            {
                return null;
            }

            return await
                _fileService.OpenReadStreamAsync(
                    fileName,
                    containerName,
                    cancellationToken);
        }

        public string GetMimeTypeFromFileName(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }

        public async Task<IStreamableKeyValuePersistenceService> GetAzureStorageReferenceService(string connectionString, string containerName)
        {
            var config = new OpsDataLoadServiceConfigSettings
            {
                ConnectionString = connectionString,
                ContainerName = containerName,
            };

            return new AzureStorageKeyValuePersistenceService(config);
        }
    }
}