using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IFileNameValidationService
    {
        Task<FileNameValidationResultModel> ValidateFileNameAsync(string collectionName, string fileName, long? fileSize, CancellationToken cancellationToken);
    }
}
