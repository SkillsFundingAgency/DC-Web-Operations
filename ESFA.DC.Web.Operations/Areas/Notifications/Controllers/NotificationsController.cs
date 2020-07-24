using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Notifications.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.Notifications;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Notifications.Controllers
{
    [Area(AreaNames.Notifications)]
    public class NotificationsController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private readonly INotificationsService _notificationsService;

        public NotificationsController(
            INotificationsService notificationsService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _notificationsService = notificationsService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = new NotificationsViewModel()
            {
                ServiceMessages = (await _notificationsService
                    .GetAllServiceMessagesAsync(cancellationToken))
                    .OrderByDescending(o => o.IsActive)
                    .ThenByDescending(t => t.StartDateTimeUtc)
            };

            return View("Index", model);
        }

        public IActionResult Manage()
        {
            throw new System.NotImplementedException();
        }
    }
}
