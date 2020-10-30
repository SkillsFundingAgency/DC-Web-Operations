using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Services.Provider;
using ESFA.DC.Web.Operations.Settings.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using CollectionType = ESFA.DC.CollectionsManagement.Models.Enums.CollectionType;

namespace ESFA.DC.Web.Operations.Services.Tests
{
    public class ManageAssignmentsServiceTests
    {
        private readonly Mock<IDateTimeProvider> _dateTimeProvider;
        private readonly Mock<IHttpClientService> _httpClientService;
        private readonly ApiSettings _apiSettings;

        public ManageAssignmentsServiceTests()
        {
            _dateTimeProvider = new Mock<IDateTimeProvider>();
            _httpClientService = new Mock<IHttpClientService>();

            _apiSettings = new ApiSettings
            {
                JobManagementApiBaseUrl = "JMUrl",
                FundingClaimsApiBaseUrl = "FCUrl"
            };
        }

        [Fact]
        public async Task GetAvailableCollectionsAsync_ExcludesJobManagmentCollectionsByDate()
        {
            // Arrange
            var currentDate = new DateTime(2020, 10, 17);

            _dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(currentDate);

            var collections = new List<Collection>
            {
                new Collection
                {
                    StartDateTimeUtc = currentDate.AddMonths(3),
                    EndDateTimeUtc = currentDate.AddMonths(5),
                    CollectionTitle = "NotYetOpen",
                    CollectionType = CollectionType.ILR.ToString()
                },
                new Collection
                {
                    StartDateTimeUtc = currentDate.AddMonths(-5),
                    EndDateTimeUtc = currentDate.AddMonths(-4),
                    CollectionTitle = "Closed",
                    CollectionType = CollectionType.ILR.ToString()
                },
                new Collection
                {
                    StartDateTimeUtc = currentDate.AddMonths(-1),
                    EndDateTimeUtc = currentDate.AddMonths(3),
                    CollectionTitle = "Open",
                    CollectionType = CollectionType.ILR.ToString()
                }
            };

            SetJobManagementCollectionMock(2021, collections);

            var _sut = new ManageAssignmentsService(Mock.Of<ILogger>(), _dateTimeProvider.Object, _apiSettings, _httpClientService.Object);

            // Act
            var result = await _sut.GetAvailableCollectionsAsync(CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal("Open", result.First().Name);
        }

        [Fact]
        public async Task GetAvailableCollectionsAsync_IncludesJobManagmentCollectionsWithinDateTolerance()
        {
            // Arrange
            var currentDate = new DateTime(2020, 10, 17);

            _dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(currentDate);

            var collections = new List<Collection>
            {
                new Collection
                {
                    StartDateTimeUtc = currentDate.AddMonths(-3),
                    EndDateTimeUtc = currentDate.AddMonths(-2),
                    CollectionTitle = "Closed but withintolerance",
                    CollectionType = CollectionType.ILR.ToString()
                },
                new Collection
                {
                    StartDateTimeUtc = currentDate.AddMonths(-2),
                    EndDateTimeUtc = currentDate.AddMonths(2),
                    CollectionTitle = "Not yet open but within tolerance",
                    CollectionType = CollectionType.ILR.ToString()
                },
            };

            SetJobManagementCollectionMock(2021, collections);

            var _sut = new ManageAssignmentsService(Mock.Of<ILogger>(), _dateTimeProvider.Object, _apiSettings, _httpClientService.Object);

            // Act
            var result = await _sut.GetAvailableCollectionsAsync(CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAvailableCollectionsAsync_ExcludesJobManagmentCollectionsByType()
        {
            // Arrange
            var currentDate = new DateTime(2020, 10, 17);

            _dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(currentDate);

            var collections = new List<Collection>
            {
                new Collection
                {
                    StartDateTimeUtc = currentDate.AddMonths(-1),
                    EndDateTimeUtc = currentDate.AddMonths(3),
                    CollectionTitle = "Open",
                    CollectionType = CollectionType.ILR.ToString()
                },
                new Collection
                {
                    StartDateTimeUtc = currentDate.AddMonths(-1),
                    EndDateTimeUtc = currentDate.AddMonths(3),
                    CollectionTitle = "Open with invalid type (funding claim)",
                    CollectionType = CollectionType.FC.ToString()
                },
                new Collection
                {
                    StartDateTimeUtc = currentDate.AddMonths(-1),
                    EndDateTimeUtc = currentDate.AddMonths(3),
                    CollectionTitle = "Open with invalid type (period end)",
                    CollectionType = CollectionType.PE.ToString()
                }
            };

            SetJobManagementCollectionMock(2021, collections);

            var _sut = new ManageAssignmentsService(Mock.Of<ILogger>(), _dateTimeProvider.Object, _apiSettings, _httpClientService.Object);

            // Act
            var result = await _sut.GetAvailableCollectionsAsync(CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal("Open", result.First().Name);
        }

        [Fact]
        public async Task GetAvailableCollectionsAsync_ExcludesFundingClaimCollectionsByDate()
        {
            // Arrange
            var currentDate = new DateTime(2020, 10, 17);

            _dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(currentDate);

            var collections = new List<FundingClaimsCollection>
            {
                new FundingClaimsCollection
                {
                    SubmissionOpenDateUtc = currentDate,
                    SubmissionCloseDateUtc = currentDate.AddDays(14),
                    CollectionName = "Open"
                },
                new FundingClaimsCollection
                {
                    SubmissionOpenDateUtc = currentDate.AddDays(-45),
                    SubmissionCloseDateUtc = currentDate.AddMonths(-20),
                    CollectionName = "Closed"
                }
            };

            SetFundingClaimsManagementCollectionMock(2021, collections);

            var _sut = new ManageAssignmentsService(Mock.Of<ILogger>(), _dateTimeProvider.Object, _apiSettings, _httpClientService.Object);

            // Act
            var result = await _sut.GetAvailableCollectionsAsync(CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal("Open", result.First().Name);
        }

        private void SetJobManagementCollectionMock(int collectionYear, List<Collection> collectionsToReturn)
        {
            _httpClientService.Setup(x => x.GetAsync<IEnumerable<Collection>>($"{_apiSettings.JobManagementApiBaseUrl}/api/collections/for-year/{collectionYear}", CancellationToken.None)).ReturnsAsync(collectionsToReturn);
        }

        private void SetFundingClaimsManagementCollectionMock(int collectionYear, List<FundingClaimsCollection> collectionsToReturn)
        {
            _httpClientService.Setup(x => x.GetAsync<IEnumerable<FundingClaimsCollection>>($"{_apiSettings.FundingClaimsApiBaseUrl}/collection/collectionYear/{collectionYear}", CancellationToken.None)).ReturnsAsync(collectionsToReturn);
        }
    }
}
