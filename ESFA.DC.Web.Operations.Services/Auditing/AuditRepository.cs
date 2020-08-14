using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.Auditing;
using ESFA.DC.Web.Operations.Topics.Data.Auditing;
using ESFA.DC.Web.Operations.Topics.Data.Auditing.Entities;

namespace ESFA.DC.Web.Operations.Services.Auditing
{
    public class AuditRepository : IAuditRepository
    {
        private readonly Func<IAuditDataContext> _context;

        public AuditRepository(Func<IAuditDataContext> context)
        {
            _context = context;
        }

        public async Task SaveAuditAsync(string user, DateTime timeStampUTC, int differentiator, string newStringValue,  string oldStringValue = null, CancellationToken cancellationToken = default)
        {
            var audit = new Audit
            {
                User = user,
                TimeStampUTC = timeStampUTC,
                NewValue = newStringValue,
                OldValue = oldStringValue,
                Differentiator = differentiator
            };
            using (var context = _context())
            {
                var pathItem = context.Audit.Add(audit);

                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
