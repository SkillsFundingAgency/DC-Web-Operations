﻿using System.Collections.Generic;
using Autofac.Features.Indexed;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Services.Reports;
using ESFA.DC.Web.Operations.Settings.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace ESFA.DC.Web.Operations.Services.Tests.Reports
{
    public class ReportServiceTests
    {
        [Fact]
        public void BuildReportFileName_Returns_FilenameAsExpected()
        {
            var operationsFileName = NewService().BuildFileName(ReportType.Operations, 1920, "R01", "ILRProviderSubmissions.xlsx");
            var fundingClaimsReportFileName = NewService().BuildFileName(ReportType.FundingClaims, 0, null, "FundingClaimsProviders1920.xlsx");
            var periodEndReportFileName = NewService().BuildFileName(ReportType.PeriodEnd, 1920, "R03", "ActCountReport.xlsx");

            operationsFileName.Should().Be("Reports/1920/R01/ILRProviderSubmissions.xlsx");
            fundingClaimsReportFileName.Should().Be("Reports/FundingClaimsProviders1920.xlsx");
            periodEndReportFileName.Should().Be("R03/ActCountReport.xlsx");
        }

        private ReportsService NewService(
            ApiSettings apiSettings = null,
            ICollectionsService collectionsService = null,
            IEnumerable<IReport> reports = null,
            IAuthorizationService authorizationService= null,
            IHttpContextAccessor httpContextAccessor = null,
            IIndex<PersistenceStorageKeys, IFileService> operationsFileService = null,
            IHttpClientService httpClientService = null)
        {
            return new ReportsService(
                apiSettings ?? new ApiSettings(),
                collectionsService ?? Mock.Of<ICollectionsService>(),
                reports,
                authorizationService ?? Mock.Of<IAuthorizationService>(),
                httpContextAccessor ?? Mock.Of<IHttpContextAccessor>(),
                operationsFileService ?? Mock.Of<IIndex<PersistenceStorageKeys, IFileService>>(),
                httpClientService ?? Mock.Of<IHttpClientService>());
        }
    }
}
