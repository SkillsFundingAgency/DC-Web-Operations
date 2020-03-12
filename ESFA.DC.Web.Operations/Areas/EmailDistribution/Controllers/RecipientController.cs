﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ESFA.DC.EmailDistribution.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Areas.EmailDistribution.ViewModels;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.EmailDistribution.Controllers
{
    [Area(AreaNames.EmailDistribution)]
    [Route(AreaNames.EmailDistribution + "/recipient")]
    public class RecipientController : BaseDistributionController
    {
        private readonly IEmailDistributionService _emailDistributionService;
        private readonly ILogger _logger;
        private readonly IJsonSerializationService _jsonSerializationService;

        public RecipientController(IEmailDistributionService emailDistributionService, IJsonSerializationService jsonSerializationService, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _emailDistributionService = emailDistributionService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new RecipientViewModel();
            model.RecipientGroups = await _emailDistributionService.GetEmailRecipientGroups();
            return View("Index", model);
        }

        [HttpPost("remove-recipient")]
        public async Task<IActionResult> Remove()
        {
            var model = new AddRemoveViewModel();
            model.Email = Request.Form["email"];
            model.AddRemove = "remove";
            int.TryParse(Request.Form["recipientId"], out var recipientId);
            int.TryParse(Request.Form["recipientGroupId"], out var recipientGroupId);
            await _emailDistributionService.RemoveRecipient(recipientId, recipientGroupId);

            return RedirectToAction("Index", "List", model);
        }

        [HttpGet("/ask-remove/{recipientId}/{recipientGroupId}")]
        public async Task<IActionResult> AskRemove(int recipientId, int recipientGroupId)
        {
            var group = await _emailDistributionService.GetGroup(recipientGroupId);
            var recipients = await _emailDistributionService.GetGroupRecipients(recipientGroupId);

            ViewData["email"] = recipients.Single(x => x.RecipientId == recipientId).EmailAddress;
            ViewData["group"] = group.GroupName;
            ViewData["recipientId"] = recipientId;
            ViewData["recipientGroupId"] = recipientGroupId;

            return View("AskRemove");
        }

        [HttpPost]
        public async Task<IActionResult> Submit(RecipientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RecipientGroups = await _emailDistributionService.GetEmailRecipientGroups();
                return View("Index", model);
            }

            var recipient = new Recipient()
            {
                EmailAddress = model.Email,
            };

            if (model.SelectedGroupIds.Any(x => x == 0))
            {
                var groups = await _emailDistributionService.GetEmailRecipientGroups();
                recipient.RecipientGroupIds = groups.ToList().Select(x => x.RecipientGroupId).ToList();
            }
            else
            {
                recipient.RecipientGroupIds = model.SelectedGroupIds.ToList();
            }

            var httpRawResponse = await _emailDistributionService.SaveRecipientAsync(recipient);

            if (httpRawResponse.StatusCode == (int)HttpStatusCode.Conflict)
            {
                var recipientGroups = _jsonSerializationService.Deserialize<List<RecipientGroup>>(httpRawResponse.Content);
                if (recipient.RecipientGroupIds.All(x => recipientGroups.Select(y => y.RecipientGroupId).Contains(x)))
                {
                    AddError(ErrorMessageKeys.Recipient_EmailFieldKey, "Email already exists in the selected distribution groups");
                    AddError(ErrorMessageKeys.ErrorSummaryKey, "Email already exists in the selected distribution groups");
                }
                else
                {
                    AddError(ErrorMessageKeys.WarningSummaryKey, "Email already associated to some of the distribution groups selected. Non-associated have been successfully added.");
                }

                model.RecipientGroups = await _emailDistributionService.GetEmailRecipientGroups();
                return View("Index", model);
            }

            var addRemoveModel = new AddRemoveViewModel
            {
                Email = model.Email,
                AddRemove = "add",
            };

            return RedirectToAction("Index", "List", addRemoveModel);
        }
    }
}