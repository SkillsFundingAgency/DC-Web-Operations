using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Storage
{
    public interface IStorageService
    {
        Task<Stream> GetFile(int collectionYear, string fileName, CancellationToken cancellationToken);

        string GetMimeTypeFromFileName(string fileName);
    }
}