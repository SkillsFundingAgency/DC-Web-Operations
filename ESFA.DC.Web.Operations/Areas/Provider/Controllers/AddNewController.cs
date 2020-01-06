using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Provider.Models;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Job;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace ESFA.DC.Web.Operations.Areas.Provider.Controllers
{
    [Area(AreaNames.Provider)]
    public class AddNewController : Controller
    {
        private const string TemplatesPath = @"\\templates";
        private const string BulkUploadFileName = @"MultipleProvidersTemplate.xlsx";
        private const string ProvidersUploadCollectionName = @"REF-OPS";
        private readonly ILogger _logger;
        private readonly ICollectionsService _collectionService;
        private readonly IStorageService _storageService;
        private readonly OpsDataLoadServiceConfigSettings _opsDataLoadServiceConfigSettings;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IJobService _jobService;
        private readonly IFileNameValidationService _fileNameValidationService;
        private readonly IAddNewProviderService _addNewProviderService;

        public AddNewController(
            ILogger logger,
            IAddNewProviderService addNewProviderService,
            ICollectionsService collectionService,
            IStorageService storageService,
            OpsDataLoadServiceConfigSettings opsDataLoadServiceConfigSettings,
            IHostingEnvironment hostingEnvironment,
            IJobService jobService,
            IFileNameValidationService fileNameValidationService)
        {
            _logger = logger;
            _collectionService = collectionService;
            _storageService = storageService;
            _opsDataLoadServiceConfigSettings = opsDataLoadServiceConfigSettings;
            _hostingEnvironment = hostingEnvironment;
            _jobService = jobService;
            _fileNameValidationService = fileNameValidationService;
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
        public async Task<IActionResult> BulkUpload(IFormFile file)
        {
            var fileName = Path.GetFileName(file?.FileName);
            var collection = await _collectionService.GetCollectionAsync(ProvidersUploadCollectionName);
            if (collection == null || !collection.IsOpen)
            {
                _logger.LogWarning($"collection {ProvidersUploadCollectionName} is not open/available, but file is being uploaded");
                throw new ArgumentOutOfRangeException(ProvidersUploadCollectionName);
            }

            var validationResult = await _fileNameValidationService.ValidateFileNameAsync(ProvidersUploadCollectionName, collection.FileNameRegex, fileName?.ToUpper(), file?.Length);

            if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            {
                ModelState.AddModelError(ErrorMessageKeys.Submission_FileFieldKey, validationResult.FieldError);
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, validationResult.SummaryError);

                _logger.LogWarning($"User uploaded invalid file with name :{fileName}");
                return View();
            }

            await (await _storageService.GetAzureStorageReferenceService(_opsDataLoadServiceConfigSettings.ConnectionString, collection.StorageReference)).SaveAsync(fileName, file?.OpenReadStream());

            await _jobService.SubmitJob(new JobSubmission
            {
                CollectionName = ProvidersUploadCollectionName,
                FileName = fileName,
                FileSizeBytes = file.Length,
                SubmittedBy = User.Name(),
                NotifyEmail = User.Email(),
                StorageReference = collection.StorageReference
            });

            return RedirectToAction("BulkUpload");
        }

        public FileResult DownloadTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(BulkUploadFileName));
            var manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
            var mimeType = "application/vnd.ms-excel";

            return File(manifestResourceStream, mimeType, BulkUploadFileName);
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
            return RedirectToAction("Index", "ManageProviders", new { ukprn = model.Ukprn });
        }
    }
}