using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Models.ReferenceData;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.ReferenceData
{
    public class ReferenceDataProcessService : IReferenceDataProcessService
    {
        private const string Api = "/api/reference-data-uploads/";

        private readonly IFileUploadJobMetaDataModelBuilderService _fileUploadJobMetaDataModelBuilderService;
        private readonly IReferenceDataServiceClient _referenceDataServiceClient;
        private readonly ILogger _logger;

        public ReferenceDataProcessService(
            IFileUploadJobMetaDataModelBuilderService fileUploadJobMetaDataModelBuilderService,
            IReferenceDataServiceClient referenceDataServiceClient,
            ILogger logger)
        {
            _fileUploadJobMetaDataModelBuilderService = fileUploadJobMetaDataModelBuilderService;
            _referenceDataServiceClient = referenceDataServiceClient;
            _logger = logger;
        }

        public async Task<ReferenceDataViewModel> GetProcessOutputsForCollectionAsync(
            string containerName,
            string collectionName,
            string reportName,
            string reportExtension,
            string fileNameFormat,
            string fileNameExtension,
            int maxRows = Constants.MaxFilesToDisplay,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var model = new ReferenceDataViewModel();

            // get job info from db
            var files = (await _referenceDataServiceClient.GetSubmittedFilesPerCollectionAsync(Api, collectionName, cancellationToken))
                .OrderByDescending(f => f.SubmissionDate)
                .Take(maxRows)
                .ToList();

            var outputFiles = await _fileUploadJobMetaDataModelBuilderService
                .BuildFileUploadJobMetaDataModelForReferenceDataProcess(
                    files,
                    collectionName,
                    containerName,
                    reportName,
                    reportExtension,
                    fileNameFormat,
                    fileNameExtension,
                    cancellationToken);

            model.Files = outputFiles;
            model.ReferenceDataCollectionName = collectionName;

            return model;
        }
    }
}
