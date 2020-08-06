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

        public AuditService(Func<IAuditDataContext> context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task CreateAudit(List<Tuple<string, object>> keyValues, string user, int differentiator)
        {
            var auditstring = BuildJobject(keyValues);
            await SaveItem(auditstring, user, differentiator);
        }

        public async Task CreateAudit(List<Tuple<string, object>> keyValuesNew, List<Tuple<string, object>> keyValuesOld, string user, int differentiator)
        {
            var auditNewString = BuildJobject(keyValuesNew);
            var auditOldString = BuildJobject(keyValuesOld);
            await SaveItem(auditNewString, auditOldString, user, differentiator);
        }

        private string BuildJobject(List<Tuple<string, object>> keyValues)
        {
            var dto = new JObject();
            foreach (var keyValue in keyValues)
            {
                dto[keyValue.Item1] = JToken.FromObject(keyValue.Item2);
            }

            return dto.ToString();
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