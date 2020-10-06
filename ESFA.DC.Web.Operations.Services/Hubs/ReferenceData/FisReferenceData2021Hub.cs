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
    public class FisReferenceData2021Hub : BaseReferenceDataHub, IReferenceDataHub
    {
        private readonly ISerialisationHelperService _serialisationHelperService;
        private readonly IHubContext<FisReferenceData2021Hub> _hubContext;
        private readonly IReferenceDataProcessService _referenceDataService;

        public FisReferenceData2021Hub(
            IHubEventBase eventBase,
            ISerialisationHelperService serialisationHelperService,
            IHubContext<FisReferenceData2021Hub> hubContext,
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
                CollectionNames.FisReferenceData2021,
                ReportTypes.FisReferenceData2021SummaryReportName,
                FileNameExtensionConsts.CSV,
                ReferenceDataOutputTypes.FisReferenceData2021ZipPreFix,
                FileNameExtensionConsts.ZIP,
                cancellationToken: cancellationToken);

            var state = _serialisationHelperService.SerialiseToCamelCase(stateModel);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", state, cancellationToken);
        }
    }
}