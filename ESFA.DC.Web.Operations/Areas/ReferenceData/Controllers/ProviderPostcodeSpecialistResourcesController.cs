using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces;
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
    [Route(AreaNames.ReferenceData + "/providerPostcodeSpecialistResources")]
    public class ProviderPostcodeSpecialistResourcesController : BaseReferenceDataController
    {
        private readonly IReferenceDataService _referenceDataService;
        private readonly IFileNameValidationServiceProvider _fileNameValidationServiceProvider;

        public ProviderPostcodeSpecialistResourcesController(
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

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Utils.Constants.ReferenceDataStorageContainerName,
                CollectionNames.ReferenceDataProviderPostcodeSpecialistResources,
                ReportTypes.ProviderPostcodeSpecialistResourcesReportName,
                cancellationToken: cancellationToken);

            return View(model);
        }

        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Index([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                return RedirectToAction("Index", new { cancellationToken = CancellationToken.None });
            }

            var fileNameValidationService = _fileNameValidationServiceProvider.GetFileNameValidationService(CollectionNames.ReferenceDataProviderPostcodeSpecialistResources);

            var validationResult = await ValidateFileName(
                fileNameValidationService,
                file.FileName,
                file.Length,
                cancellationToken);

            if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            {
                return View();
            }

            await _referenceDataService.SubmitJob(Period, CollectionNames.ReferenceDataProviderPostcodeSpecialistResources, User.Name(), User.Email(), file, CancellationToken.None);

            return RedirectToAction("Index", new { cancellationToken = CancellationToken.None });
        }

        [Route("getReportFile/{collectionName}/{fileName}/{jobId?}")]
        public async Task<FileResult> GetCollectionReportFileAsync(string collectionName, string fileName, long? jobId, CancellationToken cancellationToken)
        {
            return await GetReportFileAsync(collectionName, fileName, jobId, cancellationToken);
        }
    }
}