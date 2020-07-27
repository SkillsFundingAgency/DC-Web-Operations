using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.Notifications;
using ESFA.DC.Web.Operations.Models.Notifications;
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
            var model = await _notificationsService.GetAllNotificationMessagesAsync(cancellationToken);

            return View("Index", model);
        }

        public IActionResult Manage()
        {
            return View();
        }

        public async Task<IActionResult> Save(Notification model, CancellationToken cancellationToken)
        {
            var result = await _notificationsService.SaveNotificationAsync(cancellationToken, model);

            if (result)
            {
                return RedirectToAction("Index");
            }

            return View("Manage", model);
        }
    }
}
