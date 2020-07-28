using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Constants;
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
            if (!Validate(model))
            {
                return View("Manage", model);
            }

            var result = await _notificationsService.SaveNotificationAsync(cancellationToken, model);

            if (result)
            {
                return RedirectToAction("Index");
            }

            return View("Manage", model);
        }

        private bool Validate(Notification model)
        {
            if (string.IsNullOrEmpty(model?.Message))
            {
                ModelState.AddModelError(ErrorMessageKeys.Submission_FileFieldKey, "Please enter message");
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, "Please enter message");

                return false;
            }

            if (model.StartDate == DateTime.MinValue || model.StartTime == DateTime.MinValue)
            {
                ModelState.AddModelError(ErrorMessageKeys.Submission_FileFieldKey, "Please enter valid start date and time");
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, "Please enter valid start date and time");

                return false;
            }

            //if (model.EndDate == DateTime.MinValue || model.EndTime == DateTime.MinValue)
            //{
            //    ModelState.AddModelError(ErrorMessageKeys.Submission_FileFieldKey, "Please enter valid end date and time");
            //    ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, "Please enter valid end date and time");

            //    return false;
            //}

            //if (model.StartTime == DateTime.MinValue || model.EndTime == DateTime.MinValue)
            //{
            //    ModelState.AddModelError(ErrorMessageKeys.Submission_FileFieldKey, "Please enter valid end date and time");
            //    ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, "Please enter valid end date and time");

            //    return false;
            //}

            if (model.EndDate == DateTime.MinValue && model.EndTime != DateTime.MinValue)
            {
                ModelState.AddModelError(ErrorMessageKeys.Submission_FileFieldKey, "Please enter valid finish date");
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, "Please enter valid finish date");

                return false;
            }

            if (model.EndDate != DateTime.MinValue && model.EndTime == DateTime.MinValue)
            {
                ModelState.AddModelError(ErrorMessageKeys.Submission_FileFieldKey, "Please enter valid finish time");
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, "Please enter valid finish time");

                return false;
            }

            var startDateTime = model.StartDate.Date.Add(model.StartTime.TimeOfDay);
            var endDateTime = model.EndDate.GetValueOrDefault().Date.Add(model.EndTime.GetValueOrDefault().TimeOfDay);

            if (endDateTime != DateTime.MinValue && startDateTime > endDateTime)
            {
                ModelState.AddModelError(ErrorMessageKeys.Submission_FileFieldKey, "Start date/time can not be after finish date/time.");
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, "Start date/time can not be after finish date/time.");

                return false;
            }

            return true;
        }
    }
}
