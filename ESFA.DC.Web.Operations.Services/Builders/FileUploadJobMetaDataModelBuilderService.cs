using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.Azure.Storage.Blob;

namespace ESFA.DC.Web.Operations.Services.Builders
{
    public class FileUploadJobMetaDataModelBuilderService : IFileUploadJobMetaDataModelBuilderService
    {
        private readonly IJobStatusService _jobStatusService;
        private readonly ICloudStorageService _cloudStorageService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IFileService _fileService;

        public FileUploadJobMetaDataModelBuilderService(
            IJobStatusService jobStatusService,
            ICloudStorageService cloudStorageService,
            IDateTimeProvider dateTimeProvider,
            IIndex<PersistenceStorageKeys, IFileService> fileService)
        {
            _jobStatusService = jobStatusService;
            _cloudStorageService = cloudStorageService;
            _dateTimeProvider = dateTimeProvider;
            _fileService = fileService[PersistenceStorageKeys.DctAzureStorage];
        }

        public async Task<FileUploadJobMetaDataModel> PopulateFileUploadJobMetaDataModel(
            FileUploadJobMetaDataModel file,
            string reportName,
            string resultsReportName,
            CloudBlobContainer container,
            string periodPrefix,
            CancellationToken cancellationToken)
        {
            file.DisplayStatus = _jobStatusService.GetDisplayStatusFromJobStatus(file);

            if (file.JobStatus != JobStatuses.JobStatus_Completed)
            {
                return file;
            }

            file.ReportName = $"{reportName} {file.FileName}";

            var resultFileName = $"{periodPrefix}{file.PeriodNumber}/{file.JobId}/{resultsReportName} {Path.GetFileNameWithoutExtension(file.FileName)}.json";

            var result = await _cloudStorageService.GetSubmissionSummary(container, resultFileName, cancellationToken);
            if (result == null)
            {
                return file;
            }

            file.RecordCount = result.RecordCount;
            file.ErrorCount = result.ErrorCount;

            return file;
        }

        public async Task<FileUploadJobMetaDataModel> PopulateFileUploadJobMetaDataModelForReferenceDataUpload(
            FileUploadJobMetaDataModel file,
            string resultsReportName,
            string summaryFileName,
            CloudBlobContainer container,
            string collectionName,
            CancellationToken cancellationToken)
        {
            var clockDate = GetClockDate(file.SubmissionDate);
            file.DisplayDate = BuildDisplayDate(clockDate);

            file.FileName = Path.GetFileName(file.FileName);
            file.CollectionName = collectionName;

            file.FileName = file.FileName.Substring(file.FileName.IndexOf("/", StringComparison.InvariantCulture) + 1);

            if (file.JobStatus != JobStatuses.JobStatus_Completed)
            {
                file.DisplayStatus = _jobStatusService.GetDisplayStatusFromJobStatus(file);
                return file;
            }

            var dateSection = Path.GetFileNameWithoutExtension(file.FileName).Substring(file.FileName.IndexOf('-'));

            file.ReportName = BuildReportName(resultsReportName, dateSection, FileNameExtensionConsts.CSV);
            var resultFileName = $"{collectionName}/{file.JobId}/{summaryFileName} {Path.GetFileNameWithoutExtension(file.FileName)}.json";

            var submissionSummary = await _cloudStorageService.GetSubmissionSummary(container, resultFileName, cancellationToken);

            if (submissionSummary != null)
            {
                file.WarningCount = submissionSummary.WarningCount;
                file.RecordCount = submissionSummary.RecordCount;
                file.ErrorCount = submissionSummary.ErrorCount;
            }

            file.DisplayStatus = _jobStatusService.GetDisplayStatusFromJobStatus(file);

            return file;
        }

        public async Task<ICollection<FileUploadJobMetaDataModel>> BuildFileUploadJobMetaDataModelForReferenceDataProcess(
            List<FileUploadJobMetaDataModel> fileModels,
            string collectionName,
            string containerName,
            string reportFormat,
            string reportExtension,
            string fileNameFormat,
            string fileNameExtension,
            CancellationToken cancellationToken)
        {
            foreach (var file in fileModels)
            {
                var jobId = file.JobId.ToString();
                var clockDate = GetClockDate(file.SubmissionDate);
                file.DisplayDate = BuildDisplayDate(clockDate);

                file.CollectionName = collectionName;

                var fileName = string.Empty;

                if (fileNameFormat != null)
                {
                    var filePaths = await _fileService.GetFileReferencesAsync(containerName, $"{collectionName}/{jobId}", true, cancellationToken, false);
                    var fileNameFromStorage = filePaths?.Where(x => x.Contains(fileNameFormat)).Select(x => x.Split('/').LastOrDefault(l => l.Contains(fileNameExtension))).FirstOrDefault();
                    fileName = fileNameFromStorage;
                }

                if (file.JobStatus != JobStatuses.JobStatus_Completed)
                {
                    file.DisplayStatus = _jobStatusService.GetDisplayStatusFromJobStatus(file);
                    continue;
                }

                file.FileName = fileName;
                file.ReportName = BuildReportName(string.Concat(reportFormat, "-"), clockDate.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture), reportExtension);
                file.DisplayStatus = _jobStatusService.GetDisplayStatusFromJobStatus(file);
            }

            return fileModels;
        }

        private DateTime GetClockDate(DateTime dateTime) => _dateTimeProvider.ConvertUtcToUk(dateTime);

        private string BuildDisplayDate(DateTime dateTime)
        {
            return string.Concat(dateTime.ToString("d MMMM yyyy", CultureInfo.InvariantCulture), " at ", dateTime.ToString("h:mm tt", CultureInfo.InvariantCulture).ToLower(CultureInfo.CurrentUICulture));
        }

        private string BuildReportName(string reportName, string dateTime, string extension) => string.Concat(reportName, dateTime, extension);
    }
}