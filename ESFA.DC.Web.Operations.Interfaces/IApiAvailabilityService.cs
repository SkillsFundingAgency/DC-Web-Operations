using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IApiAvailabilityService
    {
        Task SetApiAvailabilityAsync(string apiName, string apiUpdateProcess, bool enabled, CancellationToken cancellationToken = default);
    }
}