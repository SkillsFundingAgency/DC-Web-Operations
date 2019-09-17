using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.EmailDistribution.Controllers
{
    [Area(AreaNames.EmailDistribution)]
    [Route(AreaNames.EmailDistribution + "/group")]
    public class GroupController : BaseDistributionController
    {
        private readonly IEmailDistributionService _emailDistributionService;
        private readonly ILogger _logger;

        public GroupController(IEmailDistributionService emailDistributionService, ILogger logger)
        {
            _emailDistributionService = emailDistributionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet("details/{recipientGroupId}")]
        public async Task<IActionResult> DisplayGroupRecipients(int recipientGroupId)
        {
            return View("GroupRecipients");
        }

        [HttpPost]
        public async Task<IActionResult> Submit(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                AddError(ErrorMessageKeys.Submission_FileFieldKey, "Please enter valid group name");
                AddError(ErrorMessageKeys.ErrorSummaryKey, "Please enter valid group name");

                _logger.LogWarning($"invalid group name : {groupName}");

                return View("Index");
            }

            if (await _emailDistributionService.IsDuplicateGroupName(groupName))
            {
                AddError(ErrorMessageKeys.Submission_FileFieldKey, "This mailing list name already in the system. Please enter a different name");
                AddError(ErrorMessageKeys.ErrorSummaryKey, "This mailing list name already in the system. Please enter a different name");

                _logger.LogWarning($"invalid group name : {groupName}");

                return View("Index");
            }

            await _emailDistributionService.SaveGroup(groupName);
            return RedirectToAction("Index", "List");
        }
    }
}