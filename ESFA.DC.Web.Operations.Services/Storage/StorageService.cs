using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Web.Operations.Interfaces.Storage;
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

        public string GetMimeTypeFromFileName(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}