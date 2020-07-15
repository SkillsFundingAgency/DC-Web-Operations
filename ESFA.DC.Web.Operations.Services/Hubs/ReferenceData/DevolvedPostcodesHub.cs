using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Models.ReferenceData;
using ESFA.DC.Web.Operations.Services.HubUserHandlers;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs.ReferenceData
{
    public class DevolvedPostcodesHub : BaseReferenceDataHub, IReferenceDataHub
    {
        private readonly ISerialisationHelperService _serialisationHelperService;
        private readonly IHubContext<DevolvedPostcodesHub> _hubContext;
        private readonly IReferenceDataService _referenceDataService;

        public DevolvedPostcodesHub(
            IHubEventBase eventBase,
            ISerialisationHelperService serialisationHelperService,
            IHubContext<DevolvedPostcodesHub> hubContext,
            IReferenceDataService referenceDataService,
            ILogger logger)
            : base(eventBase, logger, ReferenceDataTypes.DevolvedPostcodes)
        {
            _serialisationHelperService = serialisationHelperService;
            _hubContext = hubContext;
            _referenceDataService = referenceDataService;
        }

        public async Task SendMessage(CancellationToken cancellationToken)
        {
            if (!ReferenceDataHubUserHandler.AnyConnectionIds(ReferenceDataTypes.DevolvedPostcodes))
            {
                return;
            }

            var collection = (await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Constants.ReferenceDataStorageContainerName,
                CollectionNames.DevolvedPostcodesFullName,
                ReportTypes.DevolvedPostcodesFullNameReportName,
                cancellationToken: cancellationToken)).Files.ToList();

            collection.AddRange((await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Constants.ReferenceDataStorageContainerName,
                CollectionNames.DevolvedPostcodesSof,
                ReportTypes.DevolvedPostcodesSofReportName,
                cancellationToken: cancellationToken)).Files.ToList());

            collection.AddRange((await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Constants.ReferenceDataStorageContainerName,
                CollectionNames.DevolvedPostcodesLocalAuthority,
                ReportTypes.DevolvedPostcodesLocalAuthorityReportName,
                cancellationToken: cancellationToken)).Files.ToList());

            collection.AddRange((await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Constants.ReferenceDataStorageContainerName,
                CollectionNames.DevolvedPostcodesOnsOverride,
                ReportTypes.DevolvedPostcodesOnsOverride,
                cancellationToken: cancellationToken)).Files.ToList());

            var model = new ReferenceDataViewModel()
            {
                Files = collection
            };

            var state = _serialisationHelperService.SerialiseToCamelCase(model);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", state, cancellationToken);
        }
    }
}