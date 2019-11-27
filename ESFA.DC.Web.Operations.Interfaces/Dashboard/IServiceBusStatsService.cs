using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.Dashboard;

namespace ESFA.DC.Web.Operations.Interfaces.Dashboard
{
    public interface IServiceBusStatsService
    {
        Task<IEnumerable<ServiceBusStatusModel>> ProvideAsync(CancellationToken cancellationToken);
    }
}