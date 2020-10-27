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
    public class DevolvedPostcodesPublicationHub : BaseReferenceDataHub, IReferenceDataHub
    {
        private readonly ISerialisationHelperService _serialisationHelperService;
        private readonly IHubContext<DevolvedPostcodesPublicationHub> _hubContext;
        private readonly IReferenceDataProcessService _referenceDataService;

        public DevolvedPostcodesPublicationHub(
            IHubEventBase eventBase,
            ISerialisationHelperService serialisationHelperService,
            IHubContext<DevolvedPostcodesPublicationHub> hubContext,
            IReferenceDataProcessService referenceDataService,
            ILogger logger)
            : base(eventBase, logger, ReferenceDataTypes.FisReferenceData2021)
        {
            _serialisationHelperService = serialisationHelperService;
            _hubContext = hubContext;
            _referenceDataService = referenceDataService;
        }

        public async Task SendMessage(CancellationToken cancellationToken)
        {
            if (!ReferenceDataHubUserHandler.AnyConnectionIds(ReferenceDataTypes.FisReferenceData2021))
            {
                return;
            }

            var stateModel = await _referenceDataService.GetProcessOutputsForCollectionAsync(
                Constants.ReferenceDataStorageContainerName,
                CollectionNames.DevolvedPostcodesPublication,
                ReportTypes.DevolvedPostcodesPublicationSummaryReportName,
                FileNameExtensionConsts.CSV,
                ReferenceDataOutputTypes.DevolvedPostcodesPublicationZipPreFix,
                FileNameExtensionConsts.ZIP,
                cancellationToken: cancellationToken);

            var state = _serialisationHelperService.SerialiseToCamelCase(stateModel);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", state, cancellationToken);
        }
    }
}