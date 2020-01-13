using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Provider.Models;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Provider.Controllers
{
    [Area(AreaNames.Provider)]

    public class ManageProvidersController : Controller
    {
        private readonly ILogger _logger;
        private readonly IManageProvidersService _manageProvidersService;

        public ManageProvidersController(ILogger logger, IManageProvidersService manageProvidersService)
        {
            _logger = logger;
            _manageProvidersService = manageProvidersService;
        }

        public async Task<IActionResult> Index(long ukprn)
        {
            var viewModel = new ManageProviderViewModel();
            var provider = await _manageProvidersService.GetProvider(ukprn);
            viewModel.CollectionAssignments = await _manageProvidersService.GetProviderAssignments(ukprn);

            viewModel.ProviderName = provider.Name;
            viewModel.Ukprn = provider.Ukprn;
            viewModel.Upin = provider.Upin;
            viewModel.IsMca = provider.IsMca.GetValueOrDefault();
            if (Request.Headers["Referer"].ToString().Contains("AddNew"))
            {
                viewModel.Referer = $"/provider/AddNew?ukprn={ukprn}";
            }

            return View("Index", viewModel);
        }
    }
}