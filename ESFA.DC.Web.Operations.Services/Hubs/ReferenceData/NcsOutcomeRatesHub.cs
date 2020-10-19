using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Services.HubUserHandlers;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs.ReferenceData
{
    public class NcsOutcomeRatesHub : BaseReferenceDataHub, IReferenceDataHub
    {
        private readonly ISerialisationHelperService _serialisationHelperService;
        private readonly IHubContext<NcsOutcomeRatesHub> _hubContext;
        private readonly IReferenceDataService _referenceDataService;

        public NcsOutcomeRatesHub(
            IHubEventBase eventBase,
            ISerialisationHelperService serialisationHelperService,
            IHubContext<NcsOutcomeRatesHub> hubContext,
            IReferenceDataService referenceDataService,
            ILogger logger)
            : base(eventBase, logger, ReferenceDataTypes.NcsOutcomeRates)
        {
            _serialisationHelperService = serialisationHelperService;
            _hubContext = hubContext;
            _referenceDataService = referenceDataService;
        }

        public async Task SendMessage(CancellationToken cancellationToken)
        {
            if (!ReferenceDataHubUserHandler.AnyConnectionIds(ReferenceDataTypes.NcsOutcomeRates))
            {
                return;
            }

            var stateModel = await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Constants.ReferenceDataStorageContainerName,
                CollectionNames.NcsOutcomeRates,
                ReportTypes.NcsOutcomeRatesReportName,
                cancellationToken: cancellationToken);

            var state = _serialisationHelperService.SerialiseToCamelCase(stateModel);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", state, cancellationToken);
        }
    }
}