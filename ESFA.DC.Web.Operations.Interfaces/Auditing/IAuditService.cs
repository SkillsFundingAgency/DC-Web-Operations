using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Auditing
{
    public interface IAuditService
    {
        Task CreateAuditAsync<T>(string user, T newDto, CancellationToken cancellationToken)
            where T : class;

        Task CreateAuditAsync<T>(string user, T newDto, T oldDto, CancellationToken cancellationToken)
            where T : class;
    }
}
