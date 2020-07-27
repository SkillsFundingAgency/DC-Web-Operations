using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Notifications;
using ESFA.DC.Web.Operations.Models.ServiceMessage;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.Notifications
{
    public class NotificationsService : BaseHttpClientService, INotificationsService
    {
        private readonly string _baseUrl;
        private readonly IDateTimeProvider _dateTimeProvider;

        public NotificationsService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient,
            IDateTimeProvider dateTimeProvider)
        : base(routeFactory, jsonSerializationService, httpClient)
        {
            _dateTimeProvider = dateTimeProvider;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<IEnumerable<ServiceMessageDto>> GetAllServiceMessagesAsync(CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}/api/service-message/all";

            var data = await GetAsync<IEnumerable<Jobs.Model.ServiceMessageDto>>(url, cancellationToken);

            var result = new List<ServiceMessageDto>();

            result.AddRange(data.Select(ConvertToOperationsServiceMessageDto));

            return result;
        }

        private ServiceMessageDto ConvertToOperationsServiceMessageDto(Jobs.Model.ServiceMessageDto serviceMessageDto)
        {
            var now = _dateTimeProvider.GetNowUtc();

            return new ServiceMessageDto()
            {
                Id = serviceMessageDto.Id,
                Headline = serviceMessageDto.Headline,
                Message = serviceMessageDto.Message,
                StartDateTimeUtc = serviceMessageDto.StartDateTimeUtc,
                EndDateTimeUtc = serviceMessageDto.EndDateTimeUtc,
                IsEnabled = serviceMessageDto.IsEnabled,
                IsActive = serviceMessageDto.StartDateTimeUtc <= now && serviceMessageDto.EndDateTimeUtc >= now
            };
        }
    }
}
