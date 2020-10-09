using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.ReferenceData;
using ESFA.DC.Web.Operations.Services.ReferenceData;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Web.Operations.Services.Tests.ReferenceData
{
    public class ReferenceDataProcessServiceTests
    {
        [Fact]
        public async Task GetProcessOutputsForCollectionAsync()
        {
            var cancellationToken = CancellationToken.None;
            var submissionDateUtc = new DateTime(2020, 8, 1, 9, 0, 0);
            var clockDate = new DateTime(2020, 8, 1, 8, 0, 0);

            var collectionName = "CollectionName";
            var containerName = "Container";
            var reportFormat = "ReportPreFix";
            var reportExtension = ".csv";
            var fileNameFormat = "FileNamePreFix";
            var fileNameExtension = ".zip";

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

            var builtFileModels = new List<FileUploadJobMetaDataModel>
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

            var refDataServiceClient = new Mock<IReferenceDataServiceClient>();
            refDataServiceClient.Setup(x => x.GetSubmittedFilesPerCollectionAsync(It.IsAny<string>(), collectionName, cancellationToken)).ReturnsAsync(fileModels);

            var modelBuilderService = new Mock<IFileUploadJobMetaDataModelBuilderService>();
            modelBuilderService.Setup(x => x.BuildFileUploadJobMetaDataModelForReferenceDataProcess(
                fileModels,
                collectionName,
                containerName,
                reportFormat,
                reportExtension,
                fileNameFormat,
                fileNameExtension,
                cancellationToken)).ReturnsAsync(builtFileModels);


            var viewModel = await NewService(modelBuilderService.Object, refDataServiceClient.Object)
                .GetProcessOutputsForCollectionAsync(
                containerName,
                collectionName,
                reportFormat,
                reportExtension,
                fileNameFormat,
                fileNameExtension);

            var expectedModel = new ReferenceDataViewModel
            {
                ReferenceDataCollectionName = collectionName,
                Files = builtFileModels
            };

            viewModel.Should().BeEquivalentTo(expectedModel);
        }

        private ReferenceDataProcessService NewService(
            IFileUploadJobMetaDataModelBuilderService fileUploadJobMetaDataModelBuilderService,
            IReferenceDataServiceClient referenceDataServiceClient)
        {
            return new ReferenceDataProcessService(
                fileUploadJobMetaDataModelBuilderService,
                referenceDataServiceClient,
                Mock.Of<ILogger>());
        }
    }
}
