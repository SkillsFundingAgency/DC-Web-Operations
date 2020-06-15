using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Services.HubUserHandlers;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs.ReferenceData
{
    public class ConditionOfFundingRemovalHub : BaseReferenceDataHub, IReferenceDataHub
    {
        private readonly IHubContext<ConditionOfFundingRemovalHub> _hubContext;
        private readonly IReferenceDataService _referenceDataService;

        public ConditionOfFundingRemovalHub(
            IHubEventBase eventBase,
            ISerialisationHelperService serialisationHelperService,
            IHubContext<ConditionOfFundingRemovalHub> hubContext,
            IReferenceDataService referenceDataService,
            ILogger logger)
        : base(serialisationHelperService, eventBase, logger)
        {
            _hubContext = hubContext;
            _referenceDataService = referenceDataService;
        }

        public override Task OnConnectedAsync()
        {
            ReferenceDataHubUserHandler.CoFRConnectedIds.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ReferenceDataHubUserHandler.CoFRConnectedIds.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CancellationToken cancellationToken)
        {
            if (!ReferenceDataHubUserHandler.CoFRConnectedIds.Any())
            {
                return;
            }

            var stateModel = await _referenceDataService.GetSubmissionsPerCollectionAsync(
                    CollectionNames.ReferenceDataConditionsOfFundingRemoval,
                    ReportTypes.ConditionOfFundingRemovalReportName,
                    cancellationToken);

            var state = SerialiseToCamelCase(stateModel);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", state, cancellationToken);
        }
    }
}