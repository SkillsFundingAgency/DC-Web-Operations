using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.RuleValidation.Models;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.ValidationRules;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ESFA.DC.Web.Operations.Areas.RuleValidation.Controllers
{
    [Area(AreaNames.RuleValidation)]
    [Route(AreaNames.RuleValidation)]
    public class RulevalidationController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICollectionsService _collectionsService;
        private readonly IValidationRulesService _validationRulesService;

        public RulevalidationController(
            ILogger logger,
            ICollectionsService collectionsService,
            IValidationRulesService validationRulesService)
        {
            _logger = logger;
            _collectionsService = collectionsService;
            _validationRulesService = validationRulesService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var model = new RuleSearchViewModel();
            var collectionYears = await _collectionsService.GetAvailableCollectionYears();
            model.CollectionYears = collectionYears.OrderByDescending(x => x).ToList();
            model.Rules = await _validationRulesService.GetValidationRules(model.CollectionYears.ElementAt(0));
            ViewData["years"] = model.CollectionYears.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            return View(model);
        }

        [HttpPost("")]
        [HttpPost("Index")]
        public async Task<IActionResult> Index(int year)
        {
            var model = new RuleSearchViewModel();
            var collectionYears = await _collectionsService.GetAvailableCollectionYears();
            model.CollectionYears = collectionYears.OrderByDescending(x => x).ToList();
            model.Rules = await _validationRulesService.GetValidationRules(year);
            ViewData["years"] = model.CollectionYears.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            return View(model);
        }

        [HttpGet("ValidationRulesReport")]
        public async Task<IActionResult> ValidationRulesReport(int year, string rule)
        {
            var model = new RuleSearchViewModel();
            var collectionYears = await _collectionsService.GetAvailableCollectionYears();
            model.CollectionYears = collectionYears.OrderByDescending(x => x).ToList();
            model.Rules = await _validationRulesService.GetValidationRules(year);
            ViewData["years"] = model.CollectionYears.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            return View("Index", model);
        }
    }
}
