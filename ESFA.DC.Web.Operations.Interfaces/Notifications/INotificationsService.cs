using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.ServiceMessage;

namespace ESFA.DC.Web.Operations.Interfaces.Notifications
{
    public interface INotificationsService
    {
        Task<IEnumerable<ServiceMessageDto>> GetAllServiceMessagesAsync(CancellationToken cancellationToken);
    }
}
