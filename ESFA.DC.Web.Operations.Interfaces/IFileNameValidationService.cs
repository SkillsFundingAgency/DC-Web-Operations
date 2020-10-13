using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IFileNameValidationService
    {
        string CollectionNames { get; }

        Task<FileNameValidationResultModel> ValidateFileNameAsync(string collectionName, string fileName, string fileNameFormat, long? fileSize, CancellationToken cancellationToken);
    }
}
