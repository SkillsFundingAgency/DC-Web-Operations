using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Auditing
{
    public interface IAuditService
    {
        Task CreateAuditAsync<T>(string user, T newDto, T oldDto, CancellationToken cancellationToken = default(CancellationToken));

        Task CreateAuditAsync<T>(string user, T newDto, CancellationToken cancellationToken = default(CancellationToken));
    }
}
