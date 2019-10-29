﻿using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.EmailDistribution.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.EmailDistribution.ViewModels;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

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
            var model = new RecipientViewModel();
            model.RecipientGroups = await _emailDistributionService.GetEmailRecipientGroups();
            return View("Index", model);
        }

        [HttpPost("remove-recipient")]
        public async Task<IActionResult> Remove()
        {
            var email = Request.Form["email"];
            int.TryParse(Request.Form["recipientId"], out var recipientId);
            int.TryParse(Request.Form["recipientGroupId"], out var recipientGroupId);

            await _emailDistributionService.RemoveRecipient(recipientId, recipientGroupId);

            return View("ConfirmRemove", new Recipient() { EmailAddress = email });
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

            await _emailDistributionService.SaveRecipient(recipient);
            return View("ConfirmAdd", recipient);
        }
    }
}