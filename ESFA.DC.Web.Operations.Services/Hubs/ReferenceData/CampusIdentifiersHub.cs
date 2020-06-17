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
    public class CampusIdentifiersHub : BaseReferenceDataHub, IReferenceDataHub
    {
        private readonly ISerialisationHelperService _serialisationHelperService;
        private readonly IHubContext<CampusIdentifiersHub> _hubContext;
        private readonly IReferenceDataService _referenceDataService;

        public CampusIdentifiersHub(
            IHubEventBase eventBase,
            ISerialisationHelperService serialisationHelperService,
            IHubContext<CampusIdentifiersHub> hubContext,
            IReferenceDataService referenceDataService,
            ILogger logger)
            : base(eventBase, logger, ReferenceDataTypes.CampusIdentifier)
        {
            _serialisationHelperService = serialisationHelperService;
            _hubContext = hubContext;
            _referenceDataService = referenceDataService;
        }

        public async Task SendMessage(CancellationToken cancellationToken)
        {
            if (!ReferenceDataHubUserHandler.AnyConnectionIds(ReferenceDataTypes.CampusIdentifier))
            {
                return;
            }

            var stateModel = await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Constants.ReferenceDataStorageContainerName,
                CollectionNames.ReferenceDataCampusIdentifiers,
                ReportTypes.CampusIdentifiersReportName,
                cancellationToken);

            var state = _serialisationHelperService.SerialiseToCamelCase(stateModel);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", state, cancellationToken);
        }
    }
}