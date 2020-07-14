using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.ReferenceData;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    [Area(AreaNames.ReferenceData)]
    [Route(AreaNames.ReferenceData + "/devolvedPostcodes")]
    public class DevolvedPostcodesController : BaseReferenceDataController
    {
        private readonly IReferenceDataService _referenceDataService;
        private readonly IFileNameValidationServiceProvider _fileNameValidationServiceProvider;

        public DevolvedPostcodesController(
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
            var collection = (await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Utils.Constants.ReferenceDataStorageContainerName,
                CollectionNames.DevolvedPostcodesFullName,
                ReportTypes.DevolvedPostcodesFullNameReportName,
                cancellationToken: cancellationToken)).Files.ToList();

            collection.AddRange((await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Utils.Constants.ReferenceDataStorageContainerName,
                CollectionNames.DevolvedPostcodesSof,
                ReportTypes.DevolvedPostcodesSofReportName,
                cancellationToken: cancellationToken)).Files);

            collection.AddRange((await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Utils.Constants.ReferenceDataStorageContainerName,
                CollectionNames.DevolvedPostcodesLocalAuthority,
                ReportTypes.DevolvedPostcodesLocalAuthorityReportName,
                cancellationToken: cancellationToken)).Files);

            collection.AddRange((await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Utils.Constants.ReferenceDataStorageContainerName,
                CollectionNames.DevolvedPostcodesOnsOverride,
                ReportTypes.DevolvedPostcodesOnsOverride,
                cancellationToken: cancellationToken)).Files);

            var model = new ReferenceDataViewModel()
            {
                Files = collection
            };

            return View("Index", model);
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

            var fileNameValidationServices = _fileNameValidationServiceProvider.GetFileNameValidationServices(
                new[] { CollectionNames.DevolvedPostcodesFullName, CollectionNames.DevolvedPostcodesSof, CollectionNames.DevolvedPostcodesLocalAuthority, CollectionNames.DevolvedPostcodesOnsOverride });

            var collection = string.Empty;
            var model = new FileNameValidationResultModel();

            foreach (var service in fileNameValidationServices)
            {
                model = await ValidateFileName(
                    service,
                    file.FileName,
                    file.Length,
                    cancellationToken);

                if (model.ValidationResult == FileNameValidationResult.Valid || model.ValidationResult != FileNameValidationResult.InvalidFileNameFormat)
                {
                    collection = service.CollectionName;
                    break;
                }
            }

            if (model.ValidationResult != FileNameValidationResult.Valid)
            {
                return View();
            }

            await _referenceDataService.SubmitJob(Period, collection, User.Name(), User.Email(), file, cancellationToken);

            return RedirectToAction("Index");
        }

        [Route("getReportFile/{collectionName}/{fileName}/{jobId?}")]
        public async Task<FileResult> GetCollectionReportFileAsync(string collectionName, string fileName, long? jobId, CancellationToken cancellationToken)
        {
            return await GetReportFileAsync(collectionName, fileName, jobId, cancellationToken);
        }
    }
}