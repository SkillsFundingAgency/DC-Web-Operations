using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IPeriodService
    {
        Task<PathYearPeriod> ReturnPeriod(string collectionType, CancellationToken cancellationToken = default);

        Task<ReturnPeriod> GetRecentlyClosedPeriodAsync(CancellationToken cancellationToken = default);

        Task<List<ReturnPeriod>> GetOpenPeriodsAsync(CancellationToken cancellationToken = default);

        Task<IDictionary<string, int>> GetAllPeriodsAsync(string ilrCollectionType, CancellationToken cancellationToken);

        Task<List<int>> GetValidityYearsAsync(
            string collectionType,
            string collectionName,
            CancellationToken cancellationToken);
    }
}