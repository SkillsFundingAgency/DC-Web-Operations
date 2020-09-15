using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Services.Provider;
using ESFA.DC.Web.Operations.Settings.Models;
using FluentAssertions;
using Moq;
using Xunit;
using CollectionType = ESFA.DC.CollectionsManagement.Models.Enums.CollectionType;

namespace ESFA.DC.Web.Operations.Services.Tests
{
    public class ManageAssignmentsServiceTests
    {
        [Fact]
        public async Task GetProviderAssignmentsAsync()
        {
            var cancellationToken = CancellationToken.None;
            long ukprn = 12345678;
            var dateTimeNow = new DateTime(2020, 7, 1);
            var startDate1 = new DateTime(2020, 8, 1);
            DateTime? endDate1 = new DateTime(2020, 9, 1);
            var startDate2 = new DateTime(2020, 11, 1);
            DateTime? endDate2 = new DateTime(2021, 7, 1);

            var collectionAssignments = new List<OrganisationCollection>
            {
                new OrganisationCollection
                {
                    StartDate = startDate1,
                    EndDate = endDate1,
                    CollectionName = "CollectionName1",
                    CollectionId = 1,
                    CollectionType = CollectionType.ILR
                },
                new OrganisationCollection
                {
                    StartDate = startDate2,
                    EndDate = endDate2,
                    CollectionName = "CollectionName2",
                    CollectionId = 2,
                    CollectionType = CollectionType.EAS
                }
            };

            var expectedAssignments = new List<CollectionAssignment>
            {
                new CollectionAssignment
                {
                    StartDate = startDate1,
                    EndDate = endDate1,
                    Name = "CollectionName1",
                    CollectionId = 1,
                    DisplayOrder = 1,
                    ToBeDeleted = false
                }
            };

            var apiSettings = new ApiSettings
            {
                JobManagementApiBaseUrl = "Url"
            };

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(dateTimeNow);
            dateTimeProvider.Setup(x => x.ConvertUtcToUk(startDate1)).Returns(startDate1);
            dateTimeProvider.Setup(x => x.ConvertUtcToUk(endDate1 ?? dateTimeNow)).Returns(endDate1.Value);
            dateTimeProvider.Setup(x => x.ConvertUtcToUk(startDate2)).Returns(startDate2);
            dateTimeProvider.Setup(x => x.ConvertUtcToUk(endDate2 ?? dateTimeNow)).Returns(endDate2.Value);

            var httpClientService = new Mock<IHttpClientService>();
            httpClientService.Setup(x => x.GetAsync<IEnumerable<OrganisationCollection>>($"Url/api/org/assignments/{ukprn}", cancellationToken)).ReturnsAsync(collectionAssignments);

            var assignments = await NewService(apiSettings, dateTimeProvider.Object, httpClientService.Object).GetProviderAssignmentsAsync(ukprn, cancellationToken);

            assignments.Should().BeEquivalentTo(expectedAssignments);
        }

        [Fact]
        public async Task GetProviderAssignmentsAsync_NoCollectionEndDate()
        {
            var cancellationToken = CancellationToken.None;
            long ukprn = 12345678;
            var dateTimeNow = new DateTime(2020, 7, 1);
            var startDate = new DateTime(2020, 8, 1);
            DateTime? endDate = null;

            var collectionAssignments = new List<OrganisationCollection>
            {
                new OrganisationCollection
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    CollectionName = "CollectionName1",
                    CollectionId = 1,
                    CollectionType = CollectionType.ILR
                }
            };

            var expectedAssignments = new List<CollectionAssignment>
            {
                new CollectionAssignment
                {
                    StartDate = startDate,
                    EndDate = dateTimeNow,
                    Name = "CollectionName1",
                    CollectionId = 1,
                    DisplayOrder = 1,
                    ToBeDeleted = false
                }
            };

            var apiSettings = new ApiSettings
            {
                JobManagementApiBaseUrl = "Url"
            };

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(dateTimeNow);
            dateTimeProvider.Setup(x => x.ConvertUtcToUk(startDate)).Returns(startDate);
            dateTimeProvider.Setup(x => x.ConvertUtcToUk(endDate ?? dateTimeNow)).Returns(dateTimeNow);

            var httpClientService = new Mock<IHttpClientService>();
            httpClientService.Setup(x => x.GetAsync<IEnumerable<OrganisationCollection>>($"Url/api/org/assignments/{ukprn}", cancellationToken)).ReturnsAsync(collectionAssignments);

            var assignments = await NewService(apiSettings, dateTimeProvider.Object, httpClientService.Object).GetProviderAssignmentsAsync(ukprn, cancellationToken);

            assignments.Should().BeEquivalentTo(expectedAssignments);
        }

        [Fact]
        public async Task GetProviderAssignmentsAsync_DatesOutOfRange()
        {
            var cancellationToken = CancellationToken.None;
            long ukprn = 12345678;
            var dateTimeNow = new DateTime(2020, 5, 1);
            var startDate1 = new DateTime(2020, 8, 1);
            DateTime? endDate1 = new DateTime(2020, 9, 1);
            var startDate2 = new DateTime(2020, 11, 1);
            DateTime? endDate2 = new DateTime(2021, 7, 1);

            var collectionAssignments = new List<OrganisationCollection>
            {
                new OrganisationCollection
                {
                    StartDate = startDate1,
                    EndDate = endDate1,
                    CollectionName = "CollectionName1",
                    CollectionId = 1,
                    CollectionType = CollectionType.ILR
                },
                new OrganisationCollection
                {
                    StartDate = startDate2,
                    EndDate = endDate2,
                    CollectionName = "CollectionName2",
                    CollectionId = 2,
                    CollectionType = CollectionType.EAS
                }
            };

            var apiSettings = new ApiSettings
            {
                JobManagementApiBaseUrl = "Url"
            };

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(dateTimeNow);
            dateTimeProvider.Setup(x => x.ConvertUtcToUk(startDate1)).Returns(startDate1);
            dateTimeProvider.Setup(x => x.ConvertUtcToUk(endDate1 ?? dateTimeNow)).Returns(endDate1.Value);
            dateTimeProvider.Setup(x => x.ConvertUtcToUk(startDate2)).Returns(startDate2);
            dateTimeProvider.Setup(x => x.ConvertUtcToUk(endDate2 ?? dateTimeNow)).Returns(endDate2.Value);

            var httpClientService = new Mock<IHttpClientService>();
            httpClientService.Setup(x => x.GetAsync<IEnumerable<OrganisationCollection>>($"Url/api/org/assignments/{ukprn}", cancellationToken)).ReturnsAsync(collectionAssignments);

            var assignments = await NewService(apiSettings, dateTimeProvider.Object, httpClientService.Object).GetProviderAssignmentsAsync(ukprn, cancellationToken);

            assignments.Should().BeNullOrEmpty();
        }

        private ManageAssignmentsService NewService(
            ApiSettings apiSettings = null,
            IDateTimeProvider dateTimeProvider = null,
            IHttpClientService httpClientService = null)
        {
            return new ManageAssignmentsService(Mock.Of<ILogger>(), dateTimeProvider, apiSettings, httpClientService);
        }
    }
}
