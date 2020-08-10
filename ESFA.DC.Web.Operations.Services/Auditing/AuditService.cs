using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Auditing;
using ESFA.DC.Web.Operations.Models.Auditing;
using ESFA.DC.Web.Operations.Topics.Data.Auditing;
using ESFA.DC.Web.Operations.Topics.Data.Auditing.Entities;
using Newtonsoft.Json.Linq;

namespace ESFA.DC.Web.Operations.Services.Auditing
{
    public class AuditService : IAuditService
    {
        private readonly Func<IAuditDataContext> _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IJsonSerializationService _jsonSerializationService;

        public AuditService(Func<IAuditDataContext> context, IDateTimeProvider dateTimeProvider, IJsonSerializationService jsonSerializationService)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task CreateAuditAsync<T>(string user, T dto, CancellationToken cancellationToken = default(CancellationToken))
        {
            var auditstring = _jsonSerializationService.Serialize(dto);
            var differentiator = (int)DifferentiatorPath.Parse(typeof(DifferentiatorPath), typeof(T).Name);
            await SaveItem(differentiator, user, auditstring, null);
        }

        public async Task CreateAuditAsync<T>(string user, T newDto, T oldDto, CancellationToken cancellationToken = default(CancellationToken))
        {
            var auditNewString = _jsonSerializationService.Serialize(newDto);
            var auditOldString = _jsonSerializationService.Serialize(oldDto);
            var differentiator = (int)DifferentiatorPath.Parse(typeof(DifferentiatorPath), typeof(T).Name);
            await SaveItem(differentiator, user, auditNewString, auditOldString);
        }

        private async Task SaveItem(int differentiator, string user, string newValue, string oldValue)
        {
            var audit = new Audit
            {
                User = user,
                TimeStampUTC = _dateTimeProvider.GetNowUtc(),
                NewValue = newValue,
                OldValue = oldValue,
                Differentiator = differentiator,
            };
            using (var context = _context())
            {
                var pathItem = context.Audit.Add(audit);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }
    }
}