using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Interfaces.ValidationRules;
using Microsoft.AspNetCore.SignalR;
using ProviderSearchResult = ESFA.DC.Web.Operations.Models.Provider.ProviderSearchResult;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class ReportsHub : Hub
    {
        private readonly IHubContext<ReportsHub> _hubContext;
        private readonly IValidationRulesService _validationRulesService;
        private readonly ICollectionsService _collectionsService;

        public ReportsHub(
            IHubContext<ReportsHub> hubContext,
            IValidationRulesService validationRulesService,
            ICollectionsService collectionsService)
        {
            _hubContext = hubContext;
            _validationRulesService = validationRulesService;
            _collectionsService = collectionsService;
        }

        public async Task<IEnumerable<int>> GetCollectionYears()
        {
            var collectionYears = await _collectionsService.GetCollectionYearsByType(CollectionTypeConstants.Ilr);
            return collectionYears.OrderByDescending(x => x).ToList();
        }

        public async Task<IEnumerable<string>> GetValidationRules(int year)
        {
            var validationRules = await _validationRulesService.GetValidationRules(year);
            return validationRules;
        }
    }
}
