using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Auditing
{
    public interface IAuditService
    {
        Task CreateAudit(List<Tuple<string, object>> keyValues, string user, int differentiator);

        Task CreateAudit(List<Tuple<string, object>> keyValuesNew, List<Tuple<string, object>> keyValuesOld, string user, int differentiator);
    }
}
