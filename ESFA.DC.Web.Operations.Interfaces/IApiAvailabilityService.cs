using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IApiAvailabilityService
    {
        Task SetApiAvailabilityAsync(string apiName, string apiUpdateProcess, bool enabled, CancellationToken cancellationToken);
    }
}