using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.ReferenceData.Models;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    [Area(AreaNames.ReferenceData)]
    [Route(AreaNames.ReferenceData + "/FundingClaimsDates")]
    public class FundingClaimsDatesController : BaseReferenceDataController
    {
        private readonly IFundingClaimsDatesService _fundingClaimsDatesService;

        public FundingClaimsDatesController(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IFundingClaimsDatesService fundingClaimsDatesService)
            : base(storageService, logger, telemetryClient)
        {
            _fundingClaimsDatesService = fundingClaimsDatesService;
        }

        public async Task<IActionResult> Index()
        {
            FundingClaimsDatesViewModel viewModel = new FundingClaimsDatesViewModel();
            var fundingClaimsCollectionMetaDatas = await _fundingClaimsDatesService.GetFundingClaimsCollectionMetaDataAsync();

            var fundingClaimsDatesList = fundingClaimsCollectionMetaDatas.ToList();

            viewModel.CollectionYears = fundingClaimsDatesList.Select(x => x.CollectionYear).Distinct().OrderByDescending(x => x).ToList();
            viewModel.FundingClaimsDatesList = fundingClaimsDatesList.Where(x => x.CollectionYear == viewModel.CollectionYears.FirstOrDefault());

            return View(viewModel);
        }
    }
}