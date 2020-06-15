using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;
using Microsoft.Azure.Storage.Blob;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IFileUploadJobMetaDataModelBuilderService
    {
        Task<FileUploadJobMetaDataModel> PopulateFileUploadJobMetaDataModel(
            FileUploadJobMetaDataModel file,
            string reportName,
            string resultsReportName,
            CloudBlobContainer container,
            string periodPrefix,
            CancellationToken cancellationToken);

        Task<FileUploadJobMetaDataModel> PopulateFileUploadJobMetaDataModelForReferenceData(
            FileUploadJobMetaDataModel file,
            string resultsReportName,
            string summaryFileName,
            CloudBlobContainer container,
            string collectionName,
            CancellationToken cancellationToken);
    }
}