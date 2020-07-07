using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IFileNameValidationService
    {
        string CollectionName { get; }

        Task<FileNameValidationResultModel> ValidateFileNameAsync(string fileName, long? fileSize, CancellationToken cancellationToken);
    }
}
