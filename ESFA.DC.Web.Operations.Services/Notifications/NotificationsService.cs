﻿using System;
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
        : base(routeFactory, jsonSerializationService, httpClient)
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

        public async Task<bool> SaveNotificationAsync(CancellationToken cancellationToken, Notification model)
        {
            var startDateTime = model.StartDate.Date.Add(model.StartTime.TimeOfDay);
            var endDateTime = model.EndDate.Date.Add(model.EndTime.TimeOfDay);

            var data = new ServiceMessageDto
            {
                Headline = model.Headline,
                Message = model.Message,
                StartDateTimeUtc = _dateTimeProvider.ConvertUkToUtc(startDateTime),
                EndDateTimeUtc = _dateTimeProvider.ConvertUkToUtc(endDateTime),
                Id = model.Id
            };

            var response = await SendDataAsyncRawResponse($"{_baseUrl}/api/service-message", data, cancellationToken);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Web Operations. Error occured saving notification");
            }

            return response.IsSuccess;
        }

        private Notification ConvertToOperationsServiceMessageDto(Jobs.Model.ServiceMessageDto serviceMessageDto)
        {
            var now = _dateTimeProvider.GetNowUtc();

            return new Notification()
            {
                Id = serviceMessageDto.Id,
                Headline = serviceMessageDto.Headline,
                Message = serviceMessageDto.Message,
                StartDate = serviceMessageDto.StartDateTimeUtc,
                EndDate = serviceMessageDto.EndDateTimeUtc.GetValueOrDefault(),
                StartTime = serviceMessageDto.StartDateTimeUtc,
                EndTime = serviceMessageDto.EndDateTimeUtc.GetValueOrDefault(),
                IsActive = serviceMessageDto.StartDateTimeUtc <= now && serviceMessageDto.EndDateTimeUtc >= now
            };
        }
    }
}
