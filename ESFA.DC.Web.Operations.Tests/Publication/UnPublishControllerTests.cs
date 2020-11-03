using System;
using System.Collections.Generic;
using System.Threading;
using Autofac.Features.Indexed;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.Web.Operations.Areas.Publication.Controllers;
using ESFA.DC.Web.Operations.Areas.Publication.Models;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Publication;
using ESFA.DC.Web.Operations.Models.Publication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace ESFA.DC.Web.Operations.Tests.Publication
{
    public class UnPublishControllerTests
    {
        [Fact]
        public async void TestUnpublishFrmAsyncNoError()
        {
            var model = SetupModel(1920, 1, 0);
            var controller = SetupControllerWithLogger();

            var result = await controller.UnpublishAsync(CancellationToken.None, 4, "frm-1920");
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("UnpublishSuccess");
        }

        [Fact]
        public async void TestUnpublishFrmAsyncError()
        {
            var model = SetupModel(1920, 1, 0);
            var controller = SetupControllerError();

            var result = await controller.UnpublishAsync(CancellationToken.None, 0, null);
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("ErrorView");
        }

        private JobDetails SetupModel(int yearPeriod, int periodNumber, int frmJobId = 0)
        {
            return new JobDetails
            {
                PeriodNumber = periodNumber,
                CollectionName = "frm1920",
                StorageReference = "frm1920-files",
                DateTimeSubmitted = new DateTime(2020, 10, 10),
                CollectionYear = yearPeriod,
                JobId = frmJobId,
            };
        }

        private UnPublishController SetupControllerWithLogger(int periodNumber = 0, int collectionYear = 0 )
        {
            var frmServiceMock = new Mock<IReportsPublicationService>();
            frmServiceMock.Setup(x => x.RunValidationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(100);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(0, It.IsAny<CancellationToken>())).ReturnsAsync(6);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(5);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(8);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(4);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(3);
            frmServiceMock.Setup(x => x.GetFileSubmittedDetailsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync( 
                new JobDetails()
                {
                    DateTimeSubmitted = new DateTime(2000, 2, 3),
                    CollectionYear = collectionYear,
                    PeriodNumber = periodNumber,
                });

            var collectionServiceMock = new Mock<ICollectionsService>();
            collectionServiceMock.Setup(x => x.GetCollectionAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(() => new Collection()
                {
                    StorageReference = "frm1920-files"
                });

            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var logger = new Mock<ILogger>();
            var controller = new UnPublishController(logger.Object, frmServiceMock.Object, null, collectionServiceMock.Object, null);
            var mockcontext = new Mock<HttpContext>();
            var mockTempProvider = new Mock<ITempDataProvider>();
            controller.TempData = new TempDataDictionary(mockcontext.Object, mockTempProvider.Object);
            return controller;
        }
        
        private UnPublishController SetupControllerError()
        {
            var frmServiceMock = new Mock<IReportsPublicationService>();
            frmServiceMock.Setup(x => x.PublishSldAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            frmServiceMock.Setup(x => x.UnpublishSldAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var logger = new Mock<ILogger>();
            var collectionServiceMock = new Mock<ICollectionsService>();
            collectionServiceMock.Setup(x => x.GetCollectionAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(() => new Collection()
                {
                    StorageReference = "frm1920-files"
                });
            var controller = new UnPublishController(logger.Object, frmServiceMock.Object, null, collectionServiceMock.Object, null);
            var mockcontext = new Mock<HttpContext>();
            var mockTempProvider = new Mock<ITempDataProvider>();

            controller.TempData = new TempDataDictionary(mockcontext.Object, mockTempProvider.Object);
            return controller;
        }
    }
}
