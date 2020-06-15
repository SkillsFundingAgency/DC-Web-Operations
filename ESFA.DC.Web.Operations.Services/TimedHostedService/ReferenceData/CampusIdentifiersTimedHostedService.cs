using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Services.Hubs.ReferenceData;

namespace ESFA.DC.Web.Operations.Services.TimedHostedService.ReferenceData
{
    public class CampusIdentifiersTimedHostedService : BaseTimedHostedService
    {
        const string CampusIdentifiersCollection = "CampusIdentifier";
        const string CampusIdentifiersReportName = "CampusIdentifierRD-ValidationReport";

        private readonly ILogger _logger;
        private readonly IReferenceDataService _referenceDataService;
        private readonly ReferenceDataHub _referenceDataHub;

        public CampusIdentifiersTimedHostedService(
            ILogger logger,
            IReferenceDataService referenceDataService,
            IPeriodEndHubEventBase eventBase,
            ISerialisationHelperService serialisationHelperService,
            ReferenceDataHub referenceDataHub)
            : base("Reference Data", logger, serialisationHelperService)
        {
            _logger = logger;
            _referenceDataService = referenceDataService;
            _referenceDataHub = referenceDataHub;
            eventBase.PeriodEndHubCallback += RegisterClient;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                // Get state JSON.
                var model = await _referenceDataService.GetSubmissionsPerCollectionAsync(CampusIdentifiersCollection, CampusIdentifiersReportName, false, cancellationToken);

                var state = SerialiseToCamelCase(model);

                // Send JSON to clients.
                await _referenceDataHub.SendMessage(state, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(CampusIdentifiersTimedHostedService)}", ex);
            }
        }
    }
}
