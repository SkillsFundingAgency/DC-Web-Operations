﻿using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Auditing;

namespace ESFA.DC.Web.Operations.Services.Auditing
{
    public class AuditService : IAuditService
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IAuditRepository _auditRepository;
        private readonly IDifferentiatorLookupService _differentiatorLookupService;

        public AuditService(
            IDateTimeProvider dateTimeProvider,
            IJsonSerializationService jsonSerializationService,
            IAuditRepository auditRepository,
            IDifferentiatorLookupService differentiatorLookupService)
        {
            _dateTimeProvider = dateTimeProvider;
            _jsonSerializationService = jsonSerializationService;
            _auditRepository = auditRepository;
            _differentiatorLookupService = differentiatorLookupService;
        }

        public async Task CreateAuditAsync<T>(string user, T dto, CancellationToken cancellationToken)
            where T : class
        {
            await SaveItem(user, dto, cancellationToken: cancellationToken);
        }

        public async Task CreateAuditAsync<T>(string user, T newDto, T oldDto, CancellationToken cancellationToken)
            where T : class
        {
            await SaveItem(user, newDto, oldDto, cancellationToken);
        }

        private string SerializeDto<T>(T dto)
        {
            if (dto != null)
            {
                return _jsonSerializationService.Serialize(dto);
            }

            return null;
        }

        private async Task SaveItem<T>(string user, T newValue, T oldValue = null, CancellationToken cancellationToken = default)
            where T : class
        {
            var timeStampUTC = _dateTimeProvider.GetNowUtc();
            var newStringValue = SerializeDto(newValue);
            var oldStringValue = SerializeDto(oldValue);
            var differentiator = _differentiatorLookupService.DifferentiatorLookup<T>();
            await _auditRepository.SaveAuditAsync(user, timeStampUTC, differentiator, newStringValue, oldStringValue,  cancellationToken);
        }
    }
}