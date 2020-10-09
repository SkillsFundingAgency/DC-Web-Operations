using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Services.Builders;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Web.Operations.Services.Tests.Builders
{
    public class FileUploadJobMetaDataModelBuilderServiceTests
    {
        [Fact]
        public async Task BuildFileUploadJobMetaDataModelForReferenceDataProcess()
        {
            var cancellationToken = CancellationToken.None;
            var submissionDateUtc = new DateTime(2020, 8, 1, 9, 0, 0);
            var clockDate = new DateTime(2020, 8, 1, 8, 0, 0);

            var file1 = new FileUploadJobMetaDataModel
            {
                JobId = 1,
                JobStatus = 4,
                SubmissionDate = submissionDateUtc
            };

            var file2 = new FileUploadJobMetaDataModel
            {
                JobId = 2,
                JobStatus = 1,
                SubmissionDate = submissionDateUtc
            };

            var fileModels = new List<FileUploadJobMetaDataModel> { file1, file2 };

            var collectionName = "CollectionName";
            var containerName = "Container";
            var reportFormat = "ReportPreFix";
            var reportExtension = ".csv";
            var fileNameFormat = "FileNamePreFix";
            var fileNameExtension = ".zip";
            
            var jobStatusService = new Mock<IJobStatusService>();
            jobStatusService.Setup(x => x.GetDisplayStatusFromJobStatus(file1)).Returns("Completed");
            jobStatusService.Setup(x => x.GetDisplayStatusFromJobStatus(file2)).Returns("Processing");

            var cloudStorageService = new Mock<ICloudStorageService>();
            
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.ConvertUtcToUk(submissionDateUtc)).Returns(clockDate);

            var fileService = new Mock<IFileService>();
            fileService.Setup(x => x.GetFileReferencesAsync(containerName, $"{collectionName}/{1}", true, cancellationToken, false)).ReturnsAsync(new List<string> { @"Container/CollectionName/1/FileNamePreFix.1.2.202008010800.zip" });
            fileService.Setup(x => x.GetFileReferencesAsync(containerName, $"{collectionName}/{2}", true, cancellationToken, false)).ReturnsAsync(new List<string> { @"Container/CollectionName/2/FileNamePreFix.1.2.202008010800.zip" });

            var fileServiceIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            fileServiceIndex.Setup(x => x[PersistenceStorageKeys.DctAzureStorage]).Returns(fileService.Object);
            fileServiceIndex.Setup(x => x[PersistenceStorageKeys.DctAzureStorage]).Returns(fileService.Object);

            var expectedFileModels = new List<FileUploadJobMetaDataModel>
            {
                new FileUploadJobMetaDataModel
                {
                    JobId = 1,
                    JobStatus = 4,
                    SubmissionDate = submissionDateUtc,
                    DisplayDate = "1 August 2020 at 8:00 am",
                    CollectionName = collectionName,
                    FileName = "FileNamePreFix.1.2.202008010800.zip",
                    ReportName = "ReportPreFix-202008010800.csv",
                    DisplayStatus = "Completed"
                },
                new FileUploadJobMetaDataModel
                {
                    JobId = 2,
                    JobStatus = 1,
                    SubmissionDate = submissionDateUtc,
                    DisplayDate = "1 August 2020 at 8:00 am",
                    CollectionName = collectionName,
                    DisplayStatus = "Processing"
                }
            };

            var resultModel = await NewBuilder(jobStatusService.Object, cloudStorageService.Object, dateTimeProvider.Object, fileServiceIndex.Object)
                .BuildFileUploadJobMetaDataModelForReferenceDataProcess(
                fileModels,
                collectionName,
                containerName,
                reportFormat,
                reportExtension,
                fileNameFormat,
                fileNameExtension,
                cancellationToken);

            resultModel.Should().BeEquivalentTo(expectedFileModels);
        }

        private FileUploadJobMetaDataModelBuilderService NewBuilder(
            IJobStatusService jobStatusService,
            ICloudStorageService cloudStorageService,
            IDateTimeProvider dateTimeProvider,
            IIndex<PersistenceStorageKeys, IFileService> fileService)
        {
            return new FileUploadJobMetaDataModelBuilderService(jobStatusService, cloudStorageService, dateTimeProvider, fileService);
        }
    }
}
