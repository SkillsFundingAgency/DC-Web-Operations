using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Notifications;
using ESFA.DC.Web.Operations.Models.Notifications;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.Notifications
{
    public class NotificationsService : BaseHttpClientService, INotificationsService
    {
        private readonly string _baseUrl;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;

        public NotificationsService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient,
            IDateTimeProvider dateTimeProvider,
            ILogger logger)
        : base(routeFactory, jsonSerializationService, dateTimeProvider, httpClient)
        {
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<List<Notification>> GetAllNotificationMessagesAsync(CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}/api/service-message/all";

            var data = await GetAsync<IEnumerable<Jobs.Model.ServiceMessageDto>>(url, cancellationToken);

            var result = new List<Notification>();

            result.AddRange(data.Select(ConvertToOperationsServiceMessageDto));

            result = result.OrderByDescending(o => o.IsActive)
                    .ThenByDescending(t => t.StartDate)
                    .ToList();

            return result;
        }

        public async Task<Notification> GetNotificationByIdAsync(int id, CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}/api/service-message/id/{id}";

            var data = await GetAsync<ServiceMessageDto>(url, cancellationToken);

            var result = ConvertToOperationsServiceMessageDto(data);

            return result;
        }

        public async Task DeleteNotificationAsync(int id, CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}/api/service-message/{id}";
            await DeleteAsync(url, cancellationToken);
        }

        public async Task<bool> SaveNotificationAsync(CancellationToken cancellationToken, Notification model)
        {
            var startDateTime = model.StartDate.Date.Add(model.StartTime.TimeOfDay);
            var endDateTime = model.EndDate.GetValueOrDefault().Date.Add(model.EndTime.GetValueOrDefault().TimeOfDay);

            var data = new ServiceMessageDto
            {
                Headline = model.Headline,
                Message = model.Message,
                StartDateTimeUtc = _dateTimeProvider.ConvertUkToUtc(startDateTime),
                EndDateTimeUtc = endDateTime == DateTime.MinValue ? (DateTime?)null : _dateTimeProvider.ConvertUkToUtc(endDateTime),
                Id = model.Id,
                IsEnabled = true,
            };

            var response = await SendDataAsyncRawResponse($"{_baseUrl}/api/service-message", data, cancellationToken);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Web Operations. Error occured saving notification");
            }

            return response.IsSuccess;
        }

        private Notification ConvertToOperationsServiceMessageDto(ServiceMessageDto serviceMessageDto)
        {
            var now = _dateTimeProvider.GetNowUtc();

            var localStartDateTime = _dateTimeProvider.ConvertUtcToUk(serviceMessageDto.StartDateTimeUtc);
            DateTime? localEnDateTime = serviceMessageDto.EndDateTimeUtc.HasValue ?
                                  _dateTimeProvider.ConvertUtcToUk(serviceMessageDto.EndDateTimeUtc
                                      .GetValueOrDefault()) : (DateTime?)null;

            return new Notification()
            {
                Id = serviceMessageDto.Id,
                Headline = serviceMessageDto.Headline,
                Message = serviceMessageDto.Message,
                StartDate = localStartDateTime,
                EndDate = localEnDateTime,
                StartTime = localStartDateTime,
                EndTime = localEnDateTime,
                IsActive = serviceMessageDto.StartDateTimeUtc <= now && (!serviceMessageDto.EndDateTimeUtc.HasValue || serviceMessageDto.EndDateTimeUtc >= now)
            };
        }
    }
}
