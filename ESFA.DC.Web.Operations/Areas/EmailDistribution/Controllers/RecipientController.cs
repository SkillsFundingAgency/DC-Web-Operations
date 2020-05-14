using System.Collections.Generic;
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

        [HttpPost("remove-recipient/{recipientId}/{recipientGroupId}")]
        public async Task<IActionResult> Remove(int recipientId, int recipientGroupId)
        {
            var recipients = await _emailDistributionService.GetGroupRecipients(recipientGroupId);
            var recipientEmail = recipients.Single(x => x.RecipientId == recipientId).EmailAddress;
            await _emailDistributionService.RemoveRecipient(recipientId, recipientGroupId);
            return RedirectToAction("DisplayGroupRecipients", "group", new { recipientGroupId, recipientEmail });
        }

        [HttpGet("/ask-remove/{recipientId}/{recipientGroupId}")]
        public async Task<IActionResult> AskRemove(int recipientId, int recipientGroupId)
        {
            return await Remove(recipientId, recipientGroupId);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(RecipientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RecipientGroups = await _emailDistributionService.GetEmailRecipientGroups();
                return View("Index", model);
            }

            List<int> recipientGroupIds;
            if (model.SelectedGroupIds.Any(x => x == 0))
            {
                var groups = await _emailDistributionService.GetEmailRecipientGroups();
                recipientGroupIds = groups.ToList().Select(x => x.RecipientGroupId).ToList();
            }
            else
            {
                recipientGroupIds = model.SelectedGroupIds.ToList();
            }

            var newRecipients = model.Email.Split(";");
            foreach (var newRecipient in newRecipients)
            {
                var recipient = new Recipient()
                {
                    EmailAddress = newRecipient,
                    RecipientGroupIds = recipientGroupIds
                };

                var httpRawResponse = await _emailDistributionService.SaveRecipientAsync(recipient);

                if (httpRawResponse.StatusCode == (int)HttpStatusCode.Conflict)
                {
                    var recipientGroups =
                        _jsonSerializationService.Deserialize<List<RecipientGroup>>(httpRawResponse.Content);
                    if (recipient.RecipientGroupIds.All(
                        x => recipientGroups.Select(y => y.RecipientGroupId).Contains(x)))
                    {
                        AddError(ErrorMessageKeys.Recipient_EmailFieldKey, $"Email {newRecipient} already exists in the selected distribution groups");
                        AddError(ErrorMessageKeys.ErrorSummaryKey, $"Email {newRecipient} already exists in the selected distribution groups");
                    }
                    else
                    {
                        AddError(ErrorMessageKeys.WarningSummaryKey, $"Email {newRecipient} already associated to some of the distribution groups selected. Non-associated have been successfully added.");
                        model.IsAdd = true;
                    }
                }
                else
                {
                    model.IsAdd = true;
                }
            }

            model.RecipientGroups = await _emailDistributionService.GetEmailRecipientGroups();
            return View("Index", model);
        }
    }
}