using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Provider.Models;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.Job;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Provider.Controllers
{
    [Area(AreaNames.Provider)]
    public class AddNewController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICollectionsService _collectionService;
        private readonly IStorageService _storageService;
        private readonly CloudStorageSettings _cloudStorageSettings;
        private readonly IStreamableKeyValuePersistenceService _keyValuePersistenceService;
        private readonly IAddNewProviderService _addNewProviderService;

        public AddNewController(ILogger logger, IAddNewProviderService addNewProviderService, ICollectionsService collectionService, IStorageService storageService, CloudStorageSettings cloudStorageSettings)
        {
            _logger = logger;
            _collectionService = collectionService;
            _storageService = storageService;
            _cloudStorageSettings = cloudStorageSettings;
            _addNewProviderService = addNewProviderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(long? ukprn = null)
        {
            var model = new ProviderViewModel();

            if (ukprn.HasValue)
            {
                var provider = await _addNewProviderService.GetProvider(ukprn.Value);
                model.Ukprn = provider.Ukprn;
                model.ProviderName = provider.Name;
                model.Upin = provider.Upin;
                model.IsMca = provider.IsMca.GetValueOrDefault();
            }

            return View("Index", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddNewOption()
        {
            var model = new ProviderViewModel
            {
                IsSingleAddNewProviderChoice = true
            };

            return View(model);
        }

        public async Task<IActionResult> BulkUpload()
        {
            return View("BulkUpload");
        }

        [HttpPost]
        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> BulkUpload(string collectionName, IFormFile file)
        {
            var fileName = Path.GetFileName(file?.FileName);
            //var validationResult = await _fileNameValidationService.ValidateFileNameAsync(collectionName, fileName?.ToUpper(), file?.Length, Ukprn);

            //if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            //{
            //    AddError(ErrorMessageKeys.Submission_FileFieldKey, validationResult.FieldError);
            //    AddError(ErrorMessageKeys.ErrorSummaryKey, validationResult.SummaryError);

            //    Logger.LogWarning($"User uploaded invalid file with name :{fileName}");
            //    var lastSubmission = await GetLastSubmission(collectionName);
            //    return View(lastSubmission);
            //}

            long jobId;

            var collection = await _collectionService.GetCollectionAsync(collectionName);
            if (collection == null || !collection.IsOpen)
            {
                _logger.LogWarning($"collection {collectionName} is not open/available, but file is being uploaded");
                throw new ArgumentOutOfRangeException(collectionName);
            }

            // do we need a period?
            var period = 0; // await GetCurrentPeriodAsync(collectionName);

            // push file to Storage
            await (await _storageService.GetAzureStorageReferenceService(_cloudStorageSettings.ConnectionString, collection.ContainerName)).SaveAsync(fileName, file?.OpenReadStream());

            // add to the queue
            jobId = await _addNewProviderService.SubmitJob(new Job
            {
                CollectionName = collectionName,
                Ukprn = 1,
                FileName = fileName,
                FileSizeBytes = file.Length,
                SubmittedBy = User.Name(),
                Period = period,
                NotifyEmail = User.Email(),
                StorageReference = collection.ContainerName,
                CollectionYear = collection.CollectionYear,
                TermsAccepted = true
            });

            return RedirectToAction("BulkUpload");
        }

        [HttpPost]
        public async Task<IActionResult> AddNewChoiceSubmit(ProviderViewModel model)
        {
           if (model.IsSingleAddNewProviderChoice)
           {
               return RedirectToAction("Index");
           }

           return RedirectToAction("BulkUpload");
        }

        [HttpPost]
        public async Task<IActionResult> AddSingleProvider(ProviderViewModel model)
        {
            _logger.LogDebug("Entered AddSingleProvider");

            const string DuplicateOrganisation = "Duplicate Organisation exists.";

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var response = await _addNewProviderService.AddProvider(
                new Operations.Models.Provider.Provider(model.ProviderName, model.Ukprn.Value, model.Upin, model.IsMca), CancellationToken.None);

            if (response.StatusCode == 409)
            {
                ModelState.AddModelError("Summary", DuplicateOrganisation);
                return View("Index", model);
            }

            _logger.LogDebug("Exit AddSingleProvider");
            return RedirectToAction("Index", "ManageProviders", new { ukprn=model.Ukprn });
        }
    }
}