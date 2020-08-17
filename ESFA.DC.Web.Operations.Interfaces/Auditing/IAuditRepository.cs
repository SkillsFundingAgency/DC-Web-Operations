using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Auditing
{
    public interface IAuditRepository
    {
        Task SaveAuditAsync(string user, DateTime timeStampUTC, int differentiator, string newStringValue, string oldStringValue = null, CancellationToken cancellationToken = default);
    }
}
