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
                JobManagementApiBaseUrl = "Url"
            };
        }

        [Fact]
        public async Task GetAvailableCollectionsAsync_ExcludesCollectionsByDate()
        {
            // Arrange
            var currentDate = new DateTime(2020, 10, 17);

            _dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(currentDate);

            var collectionAssignments = new List<Collection>
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

            SetCollectionMock(2021, collectionAssignments);

            var _sut = new ManageAssignmentsService(Mock.Of<ILogger>(), _dateTimeProvider.Object, _apiSettings, _httpClientService.Object);

            // Act
            var result = await _sut.GetAvailableCollectionsAsync(CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal("Open", result.First().Name);
        }

        private void SetCollectionMock(int collectionYear, List<Collection> collectionsToReturn)
        {
            _httpClientService.Setup(x => x.GetAsync<IEnumerable<Collection>>($"Url/api/collections/for-year/{collectionYear}", CancellationToken.None)).ReturnsAsync(collectionsToReturn);
        }

    }
}
