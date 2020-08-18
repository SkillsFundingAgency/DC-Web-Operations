using Autofac.Features.Indexed;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Frm;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using FluentAssertions;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Areas.Publication.Controllers;
using ESFA.DC.Web.Operations.Areas.Publication.Models;
using Xunit;

namespace ESFA.DC.Web.Operations.Tests.Frm
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
            var result = controller.SelectValidate();
            var viewResult = result as ViewResult;

            viewResult.ViewName.Should().Be("SelectValidate");
        }

        [Fact]
        public async void TestValidateAsync()
        {

            var model = SetupModel(1920, 1);
            var controller = SetupController();

            var result = await controller.ValidateFrmAsync(model);
            var redirectResult = result as RedirectToActionResult;

            redirectResult.ActionName.Should().Be("HoldingPageAsync");
            redirectResult.RouteValues["FrmJobType"].Should().Be(Utils.Constants.FrmValidationKey);
            redirectResult.RouteValues["FrmJobId"].Should().Be(100);
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
            var controller = SetupControllerWithLogger();

            var result = await controller.HoldingPageAsync(model);
            var viewResult = result as ViewResult;
            var frmModel = viewResult.Model as PublicationReportModel;
            frmModel.FrmCSVValidDate.Should().Be(new DateTime(2000, 2, 3));
            frmModel.FrmPeriod.Should().Be("R01");
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

            var result = await controller.PublishFrmAsync(model);
            var redirectResult = result as RedirectToActionResult;

            redirectResult.ActionName.Should().Be("HoldingPageAsync");
            redirectResult.RouteValues["FrmJobType"].Should().Be(Utils.Constants.FrmPublishKey);
            redirectResult.RouteValues["FrmJobId"].Should().Be(100);
        }

        [Fact]
        public async void TestReportChoiceSelectionAsyncFrmReportChoiceTrue()
        {
            var model = SetupModel(1920, 1);
            model.IsFrmReportChoice = true;
            var controller = SetupController();

            var result = await controller.ReportChoiceSelectionAsync(model);
            var redirectResult = result as RedirectToActionResult;

            redirectResult.ActionName.Should().Be("SelectValidate");
  
        }

        [Fact]
        public async void TestReportChoiceSelectionAsyncNoData()
        {
            var model = SetupModel(1920, 1);
            model.IsFrmReportChoice = false;
            var controller = SetupControllerNoDataChoiceSelection();

            var result = await controller.ReportChoiceSelectionAsync(model);
            var viewResult = result as ViewResult;

            viewResult.ViewName.Should().Be("SelectUnpublish");

        }

        [Fact]
        public async void TestReportChoiceSelectionAsyncOneYear()
        {
            var model = SetupModel(1920, 1);
            model.IsFrmReportChoice = false;
            var controller = SetupControllerOneYearChoiceSelection();

            var result = await controller.ReportChoiceSelectionAsync(model);
            var viewResult = result as ViewResult;

            viewResult.ViewName.Should().Be("SelectUnpublish");

        }

        [Fact]
        public async void TestReportChoiceSelectionAsyncTwoYears()
        {
            var model = SetupModel(1920, 1);
            model.IsFrmReportChoice = false;
            var controller = SetupControllerMultipleYearsChoiceSelection();

            var result = await controller.ReportChoiceSelectionAsync(model);
            var viewResult = result as ViewResult;

            viewResult.ViewName.Should().Be("SelectUnpublish");

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

        [Fact]
        public async void TestUnpublishFrmAsyncNoError()
        {
            var model = SetupModel(1920, 1, 0);
            var controller = SetupControllerWithLogger();

            var result = await controller.UnpublishFrmAsync("1920/4");
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("UnpublishSuccess");
        }

        [Fact]
        public async void TestUnpublishFrmAsyncError()
        {
            var model = SetupModel(1920, 1, 0);
            var controller = SetupControllerError();

            var result = await controller.UnpublishFrmAsync(null);
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("ErrorView");
        }

        private PublicationReportModel SetupModel(int yearPeriod, int periodNumber, int frmJobId = 0)
        {
            return new PublicationReportModel
            {
                FrmYearPeriod = yearPeriod,
                FrmDate = DateTime.Now,
                FrmPeriodNumber = periodNumber,
                FrmJobId = frmJobId
            };
        }

        private PublicationController SetupController()
        {
            var frmServiceMock = new Mock<IFrmService>();
            frmServiceMock.Setup(x => x.RunValidationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(100);
            frmServiceMock.Setup(x => x.RunPublishAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(100);
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var controller = new PublicationController(null, frmServiceMock.Object, null, iIndex.Object, null);
            return controller;
        }

        private PublicationController SetupControllerWithLogger()
        {
            var frmServiceMock = new Mock<IFrmService>();
            frmServiceMock.Setup(x => x.RunValidationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(100);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(0, It.IsAny<CancellationToken>())).ReturnsAsync(6);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(5);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(8);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(4);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(3);
            frmServiceMock.Setup(x => x.GetFileSubmittedDateAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(new DateTime(2000, 2, 3));
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var logger = new Mock<ILogger>();
            var controller = new PublicationController(logger.Object, frmServiceMock.Object, null, iIndex.Object, null);
            var mockcontext = new Mock<HttpContext>();
            var mockTempProvider = new Mock<ITempDataProvider>();
            controller.TempData = new TempDataDictionary(mockcontext.Object, mockTempProvider.Object);
            return controller;
        }

        private PublicationController SetupControllerNoDataChoiceSelection()
        {
            var frmServiceMock = new Mock<IFrmService>();
            frmServiceMock.Setup(x => x.GetFrmReportsDataAsync()).ReturnsAsync(new List<PeriodEndCalendarYearAndPeriodModel>());
            frmServiceMock.Setup(x => x.GetLastTwoCollectionYearsAsync(It.IsAny<string>())).ReturnsAsync(new List<int>());
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var controller = new PublicationController(null, frmServiceMock.Object, null, iIndex.Object, null);
            return controller;
        }


        private PublicationController SetupControllerOneYearChoiceSelection()
        {
            var frmServiceMock = new Mock<IFrmService>();
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
            frmServiceMock.Setup(x => x.GetFrmReportsDataAsync()).ReturnsAsync(yearModelList);
            var yearList = new List<int>()
            {
                1920
            };
            frmServiceMock.Setup(x => x.GetLastTwoCollectionYearsAsync(It.IsAny<string>())).ReturnsAsync(yearList);
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var controller = new PublicationController(null, frmServiceMock.Object, null, iIndex.Object, null);
            return controller;
        }

        private PublicationController SetupControllerMultipleYearsChoiceSelection()
        {
            var frmServiceMock = new Mock<IFrmService>();
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
            frmServiceMock.Setup(x => x.GetFrmReportsDataAsync()).ReturnsAsync(yearModelList);
            var yearList = new List<int>()
            {
                1819,
                1920
            };
            frmServiceMock.Setup(x => x.GetLastTwoCollectionYearsAsync(It.IsAny<string>())).ReturnsAsync(yearList);
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var controller = new PublicationController(null, frmServiceMock.Object, null, iIndex.Object, null);
            return controller;
        }

        private PublicationController SetupControllerError()
        {
            var frmServiceMock = new Mock<IFrmService>();
            frmServiceMock.Setup(x => x.PublishSldAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            frmServiceMock.Setup(x => x.UnpublishSldAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var logger = new Mock<ILogger>();
            var controller = new PublicationController(logger.Object, frmServiceMock.Object, null, iIndex.Object, null);
            var mockcontext = new Mock<HttpContext>();
            var mockTempProvider = new Mock<ITempDataProvider>();
            controller.TempData = new TempDataDictionary(mockcontext.Object, mockTempProvider.Object);
            return controller;
        }

        private PublicationController SetupControllerWithLoggerError()
        {
            var frmServiceMock = new Mock<IFrmService>();
            frmServiceMock.Setup(x => x.RunValidationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(100);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(0, It.IsAny<CancellationToken>())).ReturnsAsync(6);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(5);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(8);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(4);
            frmServiceMock.Setup(x => x.GetFrmStatusAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(3);
            frmServiceMock.Setup(x => x.GetFileSubmittedDateAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(new DateTime(2000, 2, 3));
            frmServiceMock.Setup(x => x.PublishSldAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            var iIndex = new Mock<IIndex<PersistenceStorageKeys, IFileService>>();
            var logger = new Mock<ILogger>();
            var controller = new PublicationController(logger.Object, frmServiceMock.Object, null, iIndex.Object, null);
            var mockcontext = new Mock<HttpContext>();
            var mockTempProvider = new Mock<ITempDataProvider>();
            controller.TempData = new TempDataDictionary(mockcontext.Object, mockTempProvider.Object);
            return controller;
        }
    }
}
