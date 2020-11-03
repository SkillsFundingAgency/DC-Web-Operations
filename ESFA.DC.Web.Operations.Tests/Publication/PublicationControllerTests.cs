using System;
using System.Collections.Generic;
using System.Threading;
using Autofac.Features.Indexed;
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
    public class PublicationControllerTests
    {
        [Fact]
        public void TestIndex()
        {
            var controller = SetupController();

            var result = controller.Index();
            var viewResult = result as ViewResult;
            var frmModel = viewResult.Model as PublicationReportModel;

            frmModel.IsFrmReportChoice.Should().Be(false);
            viewResult.ViewName.Should().Be("Index");
        }

        [Fact]
        public void TestSelectValidate()
        {
            var controller = SetupController();
            var result = controller.SelectValidate().Result;
            var viewResult = result as ViewResult;

            viewResult.ViewName.Should().Be("SelectValidate");
        }

        [Fact]
        public async void TestValidateAsync()
        {

            var model = SetupValidationModel(1920, 1);
            var controller = SetupController();

            var result = await controller.ValidateFrmAsync(model);
            var redirectResult = result as RedirectToActionResult;

            redirectResult.ActionName.Should().Be("HoldingPageAsync");
            redirectResult.RouteValues["JobId"].Should().Be(100);
        }

        [Fact]
        public async void TestHoldingPageAsyncFailed()
        {
            var model = SetupModel(1920, 1, 0);
            var controller = SetupControllerWithLogger();

            var result = await controller.HoldingPageAsync(model);
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("ErrorView");
        }

        [Fact]
        public async void TestHoldingPageAsyncFailedRetry()
        {
            var model = SetupModel(1920, 1, 1);
            var controller = SetupControllerWithLogger();

            var result = await controller.HoldingPageAsync(model);
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("ErrorView");
        }

        [Fact]
        public async void TestHoldingPageAsyncWaiting()
        {
            var model = SetupModel(1920, 1, 2);
            var controller = SetupControllerWithLogger(10,1920);

            var result = await controller.HoldingPageAsync(model);
            var viewResult = result as ViewResult;
            var frmModel = viewResult.Model as JobDetails;
            frmModel.CollectionYear.Should().Be(1920);
            frmModel.PeriodNumber.Should().Be(10);
            viewResult.ViewName.Should().Be("ValidateSuccess");
        }

        [Fact]
        public async void TestHoldingPageAsyncCompletedNoError()
        {
            var model = SetupModel(1920, 1, 3);
            var controller = SetupControllerWithLogger();

            var result = await controller.HoldingPageAsync(model);
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("PublishSuccess");
        }

        [Fact]
        public async void TestHoldingPageAsyncCompletedError()
        {
            var model = SetupModel(1920, 1, 3);
            var controller = SetupControllerWithLoggerError();

            var result = await controller.HoldingPageAsync(model);
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("ErrorView");
        }

        [Fact]
        public async void TestHoldingPageAsyncOther()
        {
            var model = SetupModel(1920, 1, 4);
            var controller = SetupControllerWithLogger();

            var result = await controller.HoldingPageAsync(model);
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("HoldingPageAsync");
        }

        [Fact]
        public async void TestPublishAsync()
        {

            var model = SetupModel(1920, 1);
            var controller = SetupController();

            var result = await controller.PublishAsync(model);
            var redirectResult = result as RedirectToActionResult;

            redirectResult.ActionName.Should().Be("HoldingPageAsync");
            redirectResult.RouteValues["CollectionYear"].Should().Be(1920);
            redirectResult.RouteValues["CollectionName"].Should().Be("frm1920");
        }

        [Fact]
        public async void TestReportChoiceSelectionAsyncFrmReportChoiceTrue()
        {
            var model = SetupValidationModel(1920, 1);
            model.IsFrmReportChoice = true;
            var controller = SetupController();

            var result = await controller.ReportChoiceSelectionAsync(model);
            var redirectResult = result as RedirectToActionResult;

            redirectResult.ActionName.Should().Be("SelectValidate");
  
        }

        [Fact]
        public async void TestReportChoiceSelectionAsyncNoData()
        {
            var model = SetupValidationModel(1920, 1);
            model.IsFrmReportChoice = false;
            var controller = SetupControllerNoDataChoiceSelection();

            var result = await controller.ReportChoiceSelectionAsync(model);
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ControllerName.Should().Be("UnPublish");

        }

        [Fact]
        public async void TestReportChoiceSelectionAsyncOneYear()
        {
            var model = SetupValidationModel(1920, 1);
            model.IsFrmReportChoice = false;
            var controller = SetupControllerOneYearChoiceSelection();

            var result = await controller.ReportChoiceSelectionAsync(model);
            var redirectToActionResult = result as RedirectToActionResult;

            redirectToActionResult.ControllerName.Should().Be("UnPublish");

        }

        [Fact]
        public async void TestReportChoiceSelectionAsyncTwoYears()
        {
            var model = SetupValidationModel(1920, 1);
            model.IsFrmReportChoice = false;
            var controller = SetupControllerMultipleYearsChoiceSelection();

            var result = await controller.ReportChoiceSelectionAsync(model);
            var redirectToActionResult = result as RedirectToActionResult;

            redirectToActionResult.ControllerName.Should().Be("UnPublish");

        }

        [Fact]
        public void TestCancelFrm()
        {
            var model = SetupModel(1920, 1, 0);
            var controller = SetupController();

            var result = controller.CancelFrm();
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("CancelledFrm");
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

        private PublicationReportModel SetupValidationModel(int yearPeriod, int periodNumber, int frmJobId = 0)
        {
            return new PublicationReportModel
            {
                PublicationYearPeriod = yearPeriod,
                PublicationDate = DateTime.Now,
                PeriodNumber = periodNumber,
                FrmJobId = frmJobId
            };
        }

        private PublicationController SetupController()
        {
            var collectionServiceMock = new Mock<ICollectionsService>();
            var frmServiceMock = new Mock<IReportsPublicationService>();
            frmServiceMock.Setup(x => x.RunValidationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(100);
            frmServiceMock.Setup(x => x.RunPublishAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(100);
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var controller = new PublicationController(null, frmServiceMock.Object, null, collectionServiceMock.Object, null);
            return controller;
        }

        private PublicationController SetupControllerWithLogger(int periodNumber = 0, int collectionYear = 0 )
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
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var logger = new Mock<ILogger>();
            var controller = new PublicationController(logger.Object, frmServiceMock.Object, null, null, null);
            var mockcontext = new Mock<HttpContext>();
            var mockTempProvider = new Mock<ITempDataProvider>();
            controller.TempData = new TempDataDictionary(mockcontext.Object, mockTempProvider.Object);
            return controller;
        }

        private PublicationController SetupControllerNoDataChoiceSelection()
        {
            var frmServiceMock = new Mock<IReportsPublicationService>();
            frmServiceMock.Setup(x => x.GetReportsDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<PeriodEndCalendarYearAndPeriodModel>());
            frmServiceMock.Setup(x => x.GetLastTwoCollectionYearsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<int>());
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var controller = new PublicationController(null, frmServiceMock.Object, null, null, null);
            return controller;
        }


        private PublicationController SetupControllerOneYearChoiceSelection()
        {
            var frmServiceMock = new Mock<IReportsPublicationService>();
            var yearModelList = new List<PeriodEndCalendarYearAndPeriodModel>();
            yearModelList.Add(new PeriodEndCalendarYearAndPeriodModel
            {
                CollectionYear = 1920,
                PeriodNumber = 1
            });
            yearModelList.Add(new PeriodEndCalendarYearAndPeriodModel
            {
                CollectionYear = 1920,
                PeriodNumber = 2
            });
            frmServiceMock.Setup(x => x.GetReportsDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(yearModelList);
            var yearList = new List<int>()
            {
                1920
            };
            frmServiceMock.Setup(x => x.GetLastTwoCollectionYearsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(yearList);
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var controller = new PublicationController(null, frmServiceMock.Object, null, null, null);
            return controller;
        }

        private PublicationController SetupControllerMultipleYearsChoiceSelection()
        {
            var frmServiceMock = new Mock<IReportsPublicationService>();
            var yearModelList = new List<PeriodEndCalendarYearAndPeriodModel>();
            yearModelList.Add(new PeriodEndCalendarYearAndPeriodModel
            {
                CollectionYear = 1920,
                PeriodNumber = 1
            });
            yearModelList.Add(new PeriodEndCalendarYearAndPeriodModel
            {
                CollectionYear = 1819,
                PeriodNumber = 1
            });
            frmServiceMock.Setup(x => x.GetReportsDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(yearModelList);
            var yearList = new List<int>()
            {
                1819,
                1920
            };
            frmServiceMock.Setup(x => x.GetLastTwoCollectionYearsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(yearList);
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var controller = new PublicationController(null, frmServiceMock.Object, null, null, null);
            return controller;
        }

        private PublicationController SetupControllerError()
        {
            var frmServiceMock = new Mock<IReportsPublicationService>();
            frmServiceMock.Setup(x => x.PublishSldAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            frmServiceMock.Setup(x => x.UnpublishSldAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var logger = new Mock<ILogger>();
            var controller = new PublicationController(logger.Object, frmServiceMock.Object, null, null, null);
            var mockcontext = new Mock<HttpContext>();
            var mockTempProvider = new Mock<ITempDataProvider>();
            controller.TempData = new TempDataDictionary(mockcontext.Object, mockTempProvider.Object);
            return controller;
        }

        private PublicationController SetupControllerWithLoggerError()
        {
            var frmServiceMock = new Mock<IReportsPublicationService>();
            frmServiceMock.Setup(x => x.RunValidationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(100);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(0, It.IsAny<CancellationToken>())).ReturnsAsync(6);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(5);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(8);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(4);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(3);
            frmServiceMock.Setup(x => x.GetFileSubmittedDetailsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync( new JobDetails { DateTimeSubmitted = new DateTime(2000, 2, 3)});
            frmServiceMock.Setup(x => x.PublishSldAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var logger = new Mock<ILogger>();
            var controller = new PublicationController(logger.Object, frmServiceMock.Object, null, null, null);
            var mockcontext = new Mock<HttpContext>();
            var mockTempProvider = new Mock<ITempDataProvider>();
            controller.TempData = new TempDataDictionary(mockcontext.Object, mockTempProvider.Object);
            return controller;
        }
    }
}
