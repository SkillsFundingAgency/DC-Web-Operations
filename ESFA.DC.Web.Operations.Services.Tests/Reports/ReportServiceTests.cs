using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Authorisation;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Services.Reports;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using ReturnPeriod = ESFA.DC.CollectionsManagement.Models.ReturnPeriod;

namespace ESFA.DC.Web.Operations.Services.Tests.Reports
{
    public class ReportServiceTests
    {
        [Fact]
        public void BuildReportFileName_Returns_OperationsReportFilenameAsExpected()
        {
            var operationsFileName = NewService().BuildFileName(ReportType.Operations, 1920, "R01", "ILRProviderSubmissions.xlsx");
            operationsFileName.Should().Be("Reports/1920/R01/ILRProviderSubmissions.xlsx");
        }

        [Fact]
        public void BuildReportFileName_Returns_FundingClaimsReportFilenameAsExpected()
        {
            var fundingClaimsReportFileName = NewService().BuildFileName(ReportType.FundingClaims, 0, null, "FundingClaimsProviders1920.xlsx");
            fundingClaimsReportFileName.Should().Be("Reports/FundingClaimsProviders1920.xlsx");

        }

        [Fact]
        public void BuildReportFileName_Returns_PeriodEndReportsFilenameAsExpected()
        {
            var periodEndReportFileName = NewService().BuildFileName(ReportType.PeriodEnd, 1920, "R03", "ActCountReport.xlsx");
            periodEndReportFileName.Should().Be("R03/ActCountReport.xlsx");
        }


        [Fact]
        public async Task GetAvailableReportsAsync_Returns_ClosedReports()
        {
            var returnPeriods = new List<ReturnPeriod>
            {
                new ReturnPeriod
                {
                    CollectionYear = 2021,
                    PeriodNumber = 1,
                    IsOpen = false,
                    StartDateTimeUtc = new DateTime(2020, 08, 19, 08, 00, 00),
                    EndDateTimeUtc = new DateTime(2020, 09, 04, 17, 05, 00)
                },
                new ReturnPeriod
                {
                    CollectionYear = 2021,
                    PeriodNumber = 2,
                    IsOpen = false,
                    StartDateTimeUtc = new DateTime(2020,09, 14, 08, 00, 00),
                    EndDateTimeUtc = new DateTime(2020, 10, 06, 17, 05, 00)
                },
                new ReturnPeriod
                {
                    CollectionYear = 2021,
                    PeriodNumber = 3,
                    IsOpen = true,
                    StartDateTimeUtc = new DateTime(2020, 10, 14, 08, 00, 00),
                    EndDateTimeUtc = new DateTime(2020, 11, 04, 18, 05, 00)
                }
            };

            var closedReports = new List<IReport>
            {
                new ILRProvidersReturningFirstTimePerDayReport(),
                new ILRFileSubmissionsPerDayReport()
            };

            var collections = new List<CollectionSummary>()
            {
                new CollectionSummary()
                {
                    CollectionName = @"OP-ILRProvidersReturningFirstTimePerDay-Report2021"
                },
                new CollectionSummary()
                {
                    CollectionName = @"OP-ILRFileSubmissionsPerDay-Report2021"
                }
            };

            var periodServiceMock = new Mock<IPeriodService>();
            periodServiceMock.Setup(x => x.GetAllIlrPeriodsAsync(CancellationToken.None)).ReturnsAsync(returnPeriods);
            periodServiceMock.Setup(x => x.GetOpenPeriodsAsync(CancellationToken.None)).ReturnsAsync(returnPeriods.Where(w => w.IsOpen).ToList);

            var now = returnPeriods.Single(m => m.PeriodNumber == 1).StartDateTimeUtc.AddDays(1);

            var authorisationService = new Mock<IAuthorisationService>();
            authorisationService.Setup(x => x.IsAuthorisedForReport(It.IsAny<IReport>())).ReturnsAsync(true);

            var collectionServiceMock = new Mock<ICollectionsService>();
            collectionServiceMock.Setup(x => x.GetAllCollectionsForYear(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(collections);

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(now);

            var reports = await NewService(null, collectionServiceMock.Object, closedReports, authorisationService.Object, null, null, periodServiceMock.Object, dateTimeProviderMock.Object)
                .GetAvailableReportsAsync(2021, 1);

            reports.Count().Should().Be(closedReports.Count);
        }

        private ReportsService NewService(
            ApiSettings apiSettings = null,
            ICollectionsService collectionsService = null,
            IEnumerable<IReport> reports = null,
            IAuthorisationService authorisationService = null,
            IIndex<PersistenceStorageKeys, IFileService> operationsFileService = null,
            IHttpClientService httpClientService = null,
            IPeriodService periodService = null,
            IDateTimeProvider dateTimeProvider = null)
        {
            return new ReportsService(
                apiSettings ?? new ApiSettings(),
                collectionsService ?? Mock.Of<ICollectionsService>(),
                reports,
                authorisationService ?? Mock.Of<IAuthorisationService>(),
                operationsFileService ?? Mock.Of<IIndex<PersistenceStorageKeys, IFileService>>(),
                httpClientService ?? Mock.Of<IHttpClientService>(),
                periodService ?? Mock.Of<IPeriodService>(),
                dateTimeProvider ?? Mock.Of<IDateTimeProvider>());
        }
    }
}
