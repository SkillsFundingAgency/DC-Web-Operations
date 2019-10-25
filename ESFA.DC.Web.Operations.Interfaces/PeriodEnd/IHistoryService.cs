using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IHistoryService
    {
        Task<IEnumerable<HistoryDetail>> GetHistoryDetails(int year, CancellationToken cancellationToken = default);

        Task<IEnumerable<int>> GetCollectionYears(CancellationToken cancellationToken = default);
    }
}