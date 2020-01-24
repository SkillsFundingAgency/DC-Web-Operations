using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.EmailDistribution.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ESFA.DC.Web.Operations.Areas.EmailDistribution.Controllers
{
    [Area(AreaNames.EmailDistribution)]
    [Route(AreaNames.EmailDistribution + "/email-template")]
    [Authorize(Policy = ESFA.DC.Web.Operations.Constants.Authorization.AuthorisationPolicy.OpsPolicy)]
    public class EmailTemplateController : BaseDistributionController
    {
        private readonly IEmailDistributionService _emailDistributionService;
        private readonly ILogger _logger;

        public EmailTemplateController(IEmailDistributionService emailDistributionService, ILogger logger)
        {
            _emailDistributionService = emailDistributionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await _emailDistributionService.GetEmailTemplates();
            return View(data);
        }

        [HttpGet("/edit")]
        public async Task<IActionResult> Edit(int emailId)
        {
            var groups = await _emailDistributionService.GetEmailRecipientGroups();

            var items = groups.Select(a =>
                new SelectListItem
                {
                    Value = a.RecipientGroupId.ToString(),
                    Text = a.GroupName,
                }).ToList();

            items.Add(new SelectListItem()
            {
                Text = "Please select",
                Value = "0",
                Selected = true
            });

            ViewData["groups"] = items;

            var data = await _emailDistributionService.GetEmailTemplate(emailId);
            return View("Edit", data);
        }

        [HttpPost]
        public async Task<IActionResult> Save(EmailTemplate template)
        {
            if (template.RecipientGroup.RecipientGroupId < 1)
            {
                AddError(ErrorMessageKeys.Submission_FileFieldKey, "Please select a valid mailing list");
                AddError(ErrorMessageKeys.ErrorSummaryKey, "Please select a valid mailing list");

                _logger.LogWarning($"invalid template group name : {template.RecipientGroup.RecipientGroupId}");

                return await Edit(template.EmailId);
            }

            var data = await _emailDistributionService.SaveEmailTemplate(template);
            return RedirectToAction("Index");
        }
    }
}