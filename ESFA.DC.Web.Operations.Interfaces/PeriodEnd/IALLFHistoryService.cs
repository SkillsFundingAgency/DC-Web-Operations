using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.PeriodEnd;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IALLFHistoryService
    {
        Task<IEnumerable<ALLFHistoryDetail>> GetHistoryDetails(int year, CancellationToken cancellationToken = default);
    }
}