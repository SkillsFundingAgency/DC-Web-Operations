using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;
using Microsoft.Azure.Storage.Blob;

namespace ESFA.DC.Web.Operations.Interfaces
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

        Task<FileUploadJobMetaDataModel> PopulateFileUploadJobMetaDataModelForReferenceDataUpload(
            FileUploadJobMetaDataModel file,
            string resultsReportName,
            string summaryFileName,
            CloudBlobContainer container,
            string collectionName,
            CancellationToken cancellationToken);

        Task<ICollection<FileUploadJobMetaDataModel>> BuildFileUploadJobMetaDataModelForReferenceDataProcess(
            List<FileUploadJobMetaDataModel> files,
            string collectionName,
            string containerName,
            string reportFormat,
            string reportExtension,
            string fileNameFormat,
            string fileNameExtension,
            CancellationToken cancellationToken);
    }
}