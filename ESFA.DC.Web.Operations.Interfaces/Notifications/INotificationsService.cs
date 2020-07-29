using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.Notifications;

namespace ESFA.DC.Web.Operations.Interfaces.Notifications
{
    public interface INotificationsService
    {
        Task<List<Notification>> GetAllNotificationMessagesAsync(CancellationToken cancellationToken);

        Task<bool> SaveNotificationAsync(CancellationToken cancellationToken, Notification model);

        Task<Notification> GetNotificationByIdAsync(int id, CancellationToken cancellationToken);
    }
}
