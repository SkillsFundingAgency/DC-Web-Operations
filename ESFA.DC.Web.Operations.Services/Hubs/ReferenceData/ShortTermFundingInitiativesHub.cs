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
    public class ShortTermFundingInitiativesHub : BaseReferenceDataHub, IReferenceDataHub
    {
        private readonly ISerialisationHelperService _serialisationHelperService;
        private readonly IHubContext<ShortTermFundingInitiativesHub> _hubContext;
        private readonly IReferenceDataService _referenceDataService;

        public ShortTermFundingInitiativesHub(
            IHubEventBase eventBase,
            ISerialisationHelperService serialisationHelperService,
            IHubContext<ShortTermFundingInitiativesHub> hubContext,
            IReferenceDataService referenceDataService,
            ILogger logger)
            : base(eventBase, logger, ReferenceDataTypes.ShortTermFundingInitiatives)
        {
            _serialisationHelperService = serialisationHelperService;
            _hubContext = hubContext;
            _referenceDataService = referenceDataService;
        }

        public async Task SendMessage(CancellationToken cancellationToken)
        {
            if (!ReferenceDataHubUserHandler.AnyConnectionIds(ReferenceDataTypes.ShortTermFundingInitiatives))
            {
                return;
            }

            var stateModel = await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Constants.ReferenceDataStorageContainerName,
                CollectionNames.ShortTermFundingInitiatives,
                ReportTypes.ShortTermFundingInitiativesReportName,
                cancellationToken: cancellationToken);

            var state = _serialisationHelperService.SerialiseToCamelCase(stateModel);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", state, cancellationToken);
        }
    }
}