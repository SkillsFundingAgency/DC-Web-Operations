using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Auditing;
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

        public async Task CreateAudit<T>(T dto, string user, int differentiator)
        {
            var auditstring = BuildJobject<T>(dto);
            await SaveItem(auditstring, user, differentiator);
        }

        public async Task CreateAudit<T>(T newDto, T oldDto, string user, int differentiator)
        {
            var keyValues = new List<Tuple<string, object>>();
            var auditNewString = BuildJobject<T>(newDto);
            var auditOldString = BuildJobject<T>(oldDto);
            await SaveItem(auditNewString, auditOldString, user, differentiator);
        }

        private string BuildJobject<T>(T dto)
        {
            return _jsonSerializationService.Serialize(dto);
        }

        private async Task SaveItem(string newValue, string oldValue, string user, int differentiator)
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

        private async Task SaveItem(string newValue, string user, int differentiator)
        {
            var audit = new Audit
            {
                User = user,
                TimeStampUTC = _dateTimeProvider.GetNowUtc(),
                NewValue = newValue,
                Differentiator = differentiator,
            };
            using (var context = _context())
            {
                context.Audit.Add(audit);
                await context.SaveChangesAsync();
            }
        }
    }
}