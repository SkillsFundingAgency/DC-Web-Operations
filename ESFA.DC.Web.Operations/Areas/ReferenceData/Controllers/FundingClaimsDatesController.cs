using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.ReferenceData.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    [Area(AreaNames.ReferenceData)]
    [Route(AreaNames.ReferenceData + "/FundingClaimsDates")]
    public class FundingClaimsDatesController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private readonly IFundingClaimsDatesService _fundingClaimsDatesService;

        public FundingClaimsDatesController(
            ILogger logger,
            TelemetryClient telemetryClient,
            IFundingClaimsDatesService fundingClaimsDatesService)
            : base(logger, telemetryClient)
        {
            _fundingClaimsDatesService = fundingClaimsDatesService;
        }

        public async Task<IActionResult> Index()
        {
            FundingClaimsDatesViewModel viewModel = new FundingClaimsDatesViewModel();
            var fundingClaimsDatesModel = await _fundingClaimsDatesService.GetFundingClaimsCollectionMetaDataAsync();
            var fundingClaimsDatesList = fundingClaimsDatesModel.FundingClaimsDatesList;

            viewModel.CollectionYears = fundingClaimsDatesModel.Collections.Select(x => x.CollectionYear).Distinct().OrderByDescending(x => x).ToList();
            var selectedCollectionYear = viewModel.CollectionYears.FirstOrDefault();
            viewModel.Collections = fundingClaimsDatesModel.Collections.Where(x => x.CollectionYear == selectedCollectionYear).ToList();
            viewModel.FundingClaimsDatesList = fundingClaimsDatesList.Where(x => x.CollectionYear == selectedCollectionYear);

            return View(viewModel);
        }
    }
}