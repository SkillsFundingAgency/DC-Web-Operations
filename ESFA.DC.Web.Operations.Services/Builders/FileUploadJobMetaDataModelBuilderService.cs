using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

        public FileUploadJobMetaDataModelBuilderService(
            IJobStatusService jobStatusService,
            ICloudStorageService cloudStorageService)
        {
            _jobStatusService = jobStatusService;
            _cloudStorageService = cloudStorageService;
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

        public async Task<FileUploadJobMetaDataModel> PopulateFileUploadJobMetaDataModelForReferenceData(
            FileUploadJobMetaDataModel file,
            string resultsReportName,
            string summaryFileName,
            CloudBlobContainer container,
            string collectionName,
            CancellationToken cancellationToken)
        {
            file.DisplayDate = string.Concat(file.SubmissionDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture), " at ", file.SubmissionDate.ToString("hh:mm tt", CultureInfo.InvariantCulture).ToLower());

            file.DisplayStatus = _jobStatusService.GetDisplayStatusFromJobStatus(file);

            file.FileName = file.FileName.Substring(file.FileName.IndexOf("/", StringComparison.InvariantCulture) + 1);

            if (file.JobStatus != JobStatuses.JobStatus_Completed)
            {
                return file;
            }

            var dateSection = Path.GetFileNameWithoutExtension(file.FileName).Substring(file.FileName.IndexOf('-'));

            file.ReportName = $"{resultsReportName}{dateSection}.csv";
            var resultFileName = $"{collectionName}/{file.JobId}/{summaryFileName} {Path.GetFileNameWithoutExtension(file.FileName).Replace("RD", string.Empty)}.json";

            var result = await _cloudStorageService.GetSubmissionSummary(container, resultFileName, cancellationToken);
            if (result == null)
            {
                return file;
            }

            file.WarningCount = result.WarningCount;
            file.RecordCount = result.RecordCount;
            file.ErrorCount = result.ErrorCount;

            return file;
        }
    }
}