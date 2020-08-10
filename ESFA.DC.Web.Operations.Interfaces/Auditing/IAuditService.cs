using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Auditing
{
    public interface IAuditService
    {
        Task CreateAudit<T>(T keyValues, string user, int differentiator);

        Task CreateAudit<T>(T keyValuesNew, T keyValuesOld, string user, int differentiator);
    }
}
