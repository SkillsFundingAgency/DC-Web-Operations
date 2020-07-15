using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    [Area(AreaNames.ReferenceData)]
    [Route(AreaNames.ReferenceData + "/validationMessages2021")]
    public class ValidationMessages2021Controller : BaseReferenceDataController
    {
        private readonly IReferenceDataService _referenceDataService;
        private readonly IFileNameValidationServiceProvider _fileNameValidationServiceProvider;

        public ValidationMessages2021Controller(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IReferenceDataService referenceDataService,
            IFileNameValidationServiceProvider fileNameValidationServiceProvider)
            : base(storageService, logger, telemetryClient)
        {
            _referenceDataService = referenceDataService;
            _fileNameValidationServiceProvider = fileNameValidationServiceProvider;
        }

        public async Task<IActionResult> Index()
        {
            var cancellationToken = CancellationToken.None;

            var model = await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Utils.Constants.ReferenceDataStorageContainerName,
                CollectionNames.ReferenceDataValidationMessages2021,
                ReportTypes.ValidationMessagesReportName,
                Utils.Constants.ValidationMessagesMaxFilesToDisplay,
                cancellationToken);

            return View(model);
        }

        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Index([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                return RedirectToAction("Index");
            }

            var fileNameValidationService = _fileNameValidationServiceProvider.GetFileNameValidationService(CollectionNames.ReferenceDataValidationMessages2021);

            var validationResult = await ValidateFileName(
                fileNameValidationService,
                file.FileName,
                file.Length,
                cancellationToken);

            if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            {
                return View();
            }

            await _referenceDataService.SubmitJob(Period, CollectionNames.ReferenceDataValidationMessages2021, User.Name(), User.Email(), file, cancellationToken);

            return RedirectToAction("Index");
        }

        [Route("getReportFile/{collectionName}/{fileName}/{jobId?}")]
        public async Task<FileResult> GetCollectionReportFileAsync(string collectionName, string fileName, long? jobId, CancellationToken cancellationToken)
        {
            return await GetReportFileAsync(collectionName, fileName, jobId, cancellationToken);
        }
    }
}