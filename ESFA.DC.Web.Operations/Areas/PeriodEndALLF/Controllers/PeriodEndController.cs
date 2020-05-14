using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Models;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Job;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Controllers
{
    [Area(AreaNames.PeriodEndALLF)]
    [Route(AreaNames.PeriodEndALLF + "/periodEnd")]
    public class PeriodEndController : BaseControllerWithOpsPolicy
    {
        private readonly IALLFPeriodEndService _periodEndService;
        private readonly IPeriodService _periodService;
        private readonly IJobService _jobService;
        private readonly IStorageService _storageService;
        private readonly ICollectionsService _collectionService;
        private readonly AzureStorageSection _azureStorageConfig;
        private readonly ILogger _logger;

        public PeriodEndController(
            IALLFPeriodEndService periodEndService,
            IPeriodService periodService,
            IJobService jobService,
            IStorageService storageService,
            ICollectionsService collectionService,
            AzureStorageSection azureStorageConfig,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _periodEndService = periodEndService;
            _periodService = periodService;
            _jobService = jobService;
            _storageService = storageService;
            _collectionService = collectionService;
            _azureStorageConfig = azureStorageConfig;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var allfPeriodEndInfo = await _periodService.ReturnPeriod(CollectionTypes.ALLF, cancellationToken);

            var model = new PeriodEndViewModel
            {
                Files = (await GetSubmittedFiles(allfPeriodEndInfo, cancellationToken)).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                return View();
            }

            var allfPeriodEndInfo = await _periodService.ReturnPeriod(CollectionTypes.ALLF, cancellationToken);

            await SubmitJob(allfPeriodEndInfo.Period, file, cancellationToken);

            var model = new PeriodEndViewModel
            {
                Files = (await GetSubmittedFiles(allfPeriodEndInfo, cancellationToken)).ToList()
            };

            return View(model);
        }

        protected async Task<long> SubmitJob(int period, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                return 0;
            }

            long jobId;

            var collection = await _collectionService.GetCollectionAsync(CollectionNames.ALLFCollection);

            try
            {
                var fileName = Path.GetFileName(file.FileName);

                var storage = $@"A{period}\{fileName}";

                // push file to Storage
                await (await _storageService.GetAzureStorageReferenceService(_azureStorageConfig.ConnectionString, collection.StorageReference)).SaveAsync(storage, file.OpenReadStream());

                var job = new JobSubmission {
                    CollectionName = collection.CollectionTitle,
                    FileName = fileName,
                    FileSizeBytes = file.Length,
                    SubmittedBy = User.Name(),
                    NotifyEmail = User.Email(),
                    StorageReference = collection.StorageReference
                };

                // add to the queue
                jobId = await _jobService.SubmitJob(job, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error trying to submit ALLF file with name : {file.Name}", ex);
                throw;
            }

            return jobId;
        }

        private async Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmittedFiles(PathYearPeriod allfPeriodEndInfo, CancellationToken cancellationToken)
        {
            return (await _periodEndService.GetSubmittedFilesPerPeriodAsync(allfPeriodEndInfo.Year ?? 0, allfPeriodEndInfo.Period, cancellationToken)).ToList();
        }
    }
}