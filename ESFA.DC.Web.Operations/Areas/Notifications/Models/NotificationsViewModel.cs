using System.Collections.Generic;
using ESFA.DC.Web.Operations.Models.ServiceMessage;

namespace ESFA.DC.Web.Operations.Areas.Notifications.Models
{
    public class NotificationsViewModel
    {
        public IEnumerable<ServiceMessageDto> ServiceMessages { get; set; }
    }
}
