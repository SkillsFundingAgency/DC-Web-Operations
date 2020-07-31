using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Auditing
{
    public interface IAuditService
    {
        Task CreateAudit(dynamic audit, string user, int differentiator);

        Task CreateAudit(dynamic auditNew, dynamic auditOld, string user, int differentiator);
    }
}
