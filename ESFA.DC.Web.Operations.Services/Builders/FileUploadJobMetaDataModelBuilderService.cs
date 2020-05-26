using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.ALLF;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Builders
{
    public class FileUploadJobMetaDataModelBuilderService
    {
        private const string GenericActualsCollectionErrorReportName = "Generic Actuals Collection - Error Report";
        private const string ResultReportName = "Upload Result Report";

        private readonly IStorageService _storageService;
        private readonly ISerializationService _serializationService;

        public FileUploadJobMetaDataModelBuilderService(
            IStorageService storageService,
            ISerializationService serializationService)
        {
            _storageService = storageService;
            _serializationService = serializationService;
        }

        private async Task<FileUploadJobMetaDataModel> PopulateFileUploadJobMetaDataModel(FileUploadJobMetaDataModel file, int period, CancellationToken cancellationToken)
        {
            file.ReportName = $"{GenericActualsCollectionErrorReportName} {file.FileName}";

            var resultFileName = $@"{Constants.ALLFPeriodPrefix}{period}\{file.JobId}\{ResultReportName} {Path.GetFileNameWithoutExtension(file.FileName)}.json";
            var resultStream = await _storageService.GetFile(Constants.ALLFStorageContainerName, resultFileName, cancellationToken);

            if (resultStream == null)
            {
                return file;
            }

            var result = _serializationService.Deserialize<SubmissionSummary>(resultStream);

            if (result == null)
            {
                return file;
            }

            file.RecordCount = result.RecordCount;
            file.ErrorCount = result.ErrorCount;

            return file;
        }
    }
}