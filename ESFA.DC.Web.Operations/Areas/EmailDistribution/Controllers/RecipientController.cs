using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.EmailDistribution.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;

namespace ESFA.DC.Web.Operations.Areas.EmailDistribution.Controllers
{
    [Area(AreaNames.EmailDistribution)]
    [Route(AreaNames.EmailDistribution + "/recipient")]
    public class RecipientController : BaseDistributionController
    {
        private readonly IEmailDistributionService _emailDistributionService;
        private readonly ILogger _logger;

        public RecipientController(IEmailDistributionService emailDistributionService, ILogger logger)
        {
            _emailDistributionService = emailDistributionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _emailDistributionService.GetEmailRecipientGroups();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Submit()
        {
            var email = Request.Form["email"];
            var groups = Request.Form["groups"];

            if (string.IsNullOrEmpty(email))
            {
                AddError(ErrorMessageKeys.Submission_FileFieldKey, "Please enter valid email address");
                AddError(ErrorMessageKeys.ErrorSummaryKey, "Please enter valid email address");

                _logger.LogWarning($"invalid email address : {email}");

                var data = await _emailDistributionService.GetEmailRecipientGroups();
                return View("Index", data);
            }

            if (!groups.Any())
            {
                AddError(ErrorMessageKeys.Submission_FileFieldKey, "Please select at least one group");
                AddError(ErrorMessageKeys.ErrorSummaryKey, "Please select at least one group");

                _logger.LogWarning($"no groups selected for email address : {email}");

                var data = await _emailDistributionService.GetEmailRecipientGroups();
                return View("Index", data);
            }

            var recipient = new Recipient()
            {
                Email = email,
                RecipientGroupIds = groups.ToList().Select(x => int.Parse(x)).ToList(),
            };

            await _emailDistributionService.SaveRecipient(recipient);

            return RedirectToAction("Index", "List");
        }
    }
}