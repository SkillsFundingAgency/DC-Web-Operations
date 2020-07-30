using System;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Topics.Data.Auditing;
using ESFA.DC.Web.Operations.Topics.Data.Auditing.Entities;

namespace ESFA.DC.Web.Operations.Services.Auditing
{
    public class AuditService
    {
        private readonly Func<IAuditDataContext> _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IJsonSerializationService _jsonSerializationService;

        public AuditService(IDateTimeProvider dateTimeProvider, IJsonSerializationService jsonSerializationService)
        {
            dateTimeProvider = _dateTimeProvider;
        }

        public async Task CreateAudit(dynamic audit, string user, int differentiator)
        {
            await SaveItem(user, audit, differentiator);
        }

        public async Task CreateAudit(dynamic auditNew, dynamic auditOld, string user, int differentiator)
        {
            await SaveItem(user, auditNew, auditOld, differentiator);
        }

        private async Task SaveItem(string user, string newValue, string oldValue, int differentiator)
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
                var pathItem = context.Audit.AddAsync(audit);

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

        private async Task SaveItem(string user, string newValue, int differentiator)
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
                await context.Audit.AddAsync(audit);
                await context.SaveChangesAsync();
            }
        }
    }
}