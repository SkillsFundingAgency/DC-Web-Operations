using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.IO.Interfaces;

namespace ESFA.DC.Web.Operations.Interfaces.Storage
{
    public interface IStorageService
    {
        Task<Stream> GetFile(string containerName, string fileName, CancellationToken cancellationToken);

        string GetMimeTypeFromFileName(string fileName);

        Task<IStreamableKeyValuePersistenceService> GetAzureStorageReferenceService(string connectionString, string containerName);
    }
}