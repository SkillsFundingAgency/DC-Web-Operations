using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Services.Collections;
using ESFA.DC.Web.Operations.Settings.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Web.Operations.Tests.Collections
{
    public class CollectionsServiceTests
    {
        [Fact]
        public async Task GetReturnPeriod_Should_Return_UK_Dates()
        {
            var startDate = new DateTime(2020, 9, 30, 9, 0, 0);
            var endDate = new DateTime(2020, 9, 30, 17, 0, 0);

            var apiSettings = new ApiSettings
            {
                JobManagementApiBaseUrl = "foo"
            };

            var id = 1;
            var url = $"{apiSettings.JobManagementApiBaseUrl}/api/returnperiod/{id}";

            var returnPeriod = new CollectionsManagement.Models.ReturnPeriod
            {
                CollectionId = id,
                CollectionName = "bar",
                StartDateTimeUtc = startDate,
                EndDateTimeUtc = endDate
            };

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(m => m.ConvertUtcToUk(startDate)).Returns(startDate.AddHours(-1));
            dateTimeProviderMock.Setup(m => m.ConvertUtcToUk(endDate)).Returns(endDate.AddHours(-1));

            var httpClientServiceMock = new Mock<IHttpClientService>();
            httpClientServiceMock
                .Setup(m => m.GetAsync<CollectionsManagement.Models.ReturnPeriod>(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnPeriod);
            
            var service = GetService(apiSettings, dateTimeProviderMock.Object, null, httpClientServiceMock.Object);
            var result = await service.GetReturnPeriod(id, CancellationToken.None);

            result.Should().NotBeNull();
            result.OpenDate.Should().Be(startDate.AddHours(-1));
            result.CloseDate.Should().Be(endDate.AddHours(-1));
        }

        private ICollectionsService GetService(
            ApiSettings apiSettings = null,
            IDateTimeProvider dateTimeProvider = null,
            IEnumerable<ICollection> referenceDataCollections = null,
            IHttpClientService httpClientService = null,
            IJsonSerializationService jsonSerializationService = null)
        {
            return new CollectionsService(apiSettings, dateTimeProvider, Mock.Of<ILogger>(), referenceDataCollections, httpClientService, jsonSerializationService);
        }
    }
}