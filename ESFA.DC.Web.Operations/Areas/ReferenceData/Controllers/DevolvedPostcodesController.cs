using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.Collections;
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
        private readonly IEnumerable<ICollection> _collections;

        private readonly string[] _devolvedPostCodesCollections =
        {
            CollectionNames.DevolvedPostcodesFullName,
            CollectionNames.DevolvedPostcodesSof,
            CollectionNames.DevolvedPostcodesLocalAuthority,
            CollectionNames.DevolvedPostcodesOnsOverride
        };

        public DevolvedPostcodesController(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IReferenceDataService referenceDataService,
            IEnumerable<ICollection> collections,
            IFileNameValidationServiceProvider fileNameValidationServiceProvider)
            : base(storageService, logger, telemetryClient, fileNameValidationServiceProvider)
        {
            _referenceDataService = referenceDataService;
            _collections = collections;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var fileUploadJobs = new List<FileUploadJobMetaDataModel>();

            foreach (var collection in _devolvedPostCodesCollections)
            {
                var col = _collections.Single(s => string.Equals(collection, s.CollectionName, StringComparison.CurrentCultureIgnoreCase));

                fileUploadJobs.AddRange((await _referenceDataService.GetSubmissionsPerCollectionAsync(
                    Utils.Constants.ReferenceDataStorageContainerName,
                    col.CollectionName,
                    col.ReportName,
                    cancellationToken: cancellationToken)).Files);
            }

            var model = new ReferenceDataViewModel()
            {
                Files = fileUploadJobs
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

            var model = new FileNameValidationResultModel();
            var collectionForJob = string.Empty;

            foreach (var collection in _devolvedPostCodesCollections)
            {
                var col = _collections.Single(s =>
                    string.Equals(collection, s.CollectionName, StringComparison.CurrentCultureIgnoreCase));

                model = await ValidateFileName(
                    col,
                    file.FileName,
                    file.Length,
                    cancellationToken);

                if (model.ValidationResult == FileNameValidationResult.Valid ||
                    model.ValidationResult != FileNameValidationResult.InvalidFileNameFormat)
                {
                    collectionForJob = col.CollectionName;
                    break;
                }
            }

            if (model.ValidationResult != FileNameValidationResult.Valid)
            {
                return View();
            }

            await _referenceDataService.SubmitJobAsync(Period, collectionForJob, User.Name(), User.Email(), file, cancellationToken);

            return RedirectToAction("Index");
        }

        [Route("getReportFile/{collectionName}/{fileName}/{jobId?}")]
        public async Task<FileResult> GetCollectionReportFileAsync(string collectionName, string fileName, long? jobId, CancellationToken cancellationToken)
        {
            return await GetReportFileAsync(collectionName, fileName, jobId, cancellationToken);
        }
    }
}