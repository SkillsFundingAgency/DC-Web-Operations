using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Storage
{
    public interface IStorageService
    {
        Task<Stream> GetFile(string containerName, string fileName, CancellationToken cancellationToken);

        string GetMimeTypeFromFileName(string fileName);
    }
}