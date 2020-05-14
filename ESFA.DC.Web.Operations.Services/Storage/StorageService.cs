using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.FileService.Interface;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.ALLF;
using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.AspNetCore.StaticFiles;

namespace ESFA.DC.Web.Operations.Services.Storage
{
    public class StorageService : IStorageService
    {
        private readonly IJsonSerializationService _serializationService;
        private readonly IFileService _fileService;

        public StorageService(
            IJsonSerializationService serializationService,
            [KeyFilter(PersistenceStorageKeys.OperationsAzureStorage)] IFileService fileService)
        {
            _serializationService = serializationService;
            _fileService = fileService;
        }

        public async Task<Stream> GetFile(string containerName, string fileName, CancellationToken cancellationToken)
        {
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

        public async Task GetFilesFromFolder(string containerName, string prefix, bool includeSubFolders, CancellationToken cancellationToken)
        {
            var fileReferences = await _fileService.GetFileReferencesAsync(containerName, prefix, includeSubFolders, cancellationToken);

            foreach (var reference in fileReferences)
            {
                using (var fileStream = await GetFile(containerName, reference, cancellationToken))
                {
                    // TODO remove this stub method if not using JSON stats file
                    _serializationService.Deserialize<SubmissionSummary>(fileStream);
                }
            }
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