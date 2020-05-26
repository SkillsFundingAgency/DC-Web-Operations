using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IFileUploadJobMetaDataModelBuilderService
    {
        Task<FileUploadJobMetaDataModel> PopulateFileUploadJobMetaDataModel(FileUploadJobMetaDataModel file, int period, CancellationToken cancellationToken);
    }
}