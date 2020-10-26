using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class ValidityPeriodHub : BaseHub<ValidityPeriodHub>
    {
        private readonly ILogger _logger;
        private readonly IValidityPeriodService _validityPeriodService;

        public ValidityPeriodHub(
            IValidityPeriodHubEventBase eventBase,
            IHubContext<ValidityPeriodHub> hubContext,
            ILogger logger,
            IValidityPeriodService validityPeriodService)
            : base(eventBase, hubContext, logger)
        {
            _logger = logger;
            _validityPeriodService = validityPeriodService;
        }

        public async Task GetValidityPeriodList(int collectionYear, int period)
        {
            try
            {
                await ReceiveMessage();

                var validityPeriodData = await _validityPeriodService.GetValidityPeriodList(collectionYear, period);

                await SendMessage(validityPeriodData);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task GetValidityStructure(int collectionYear, int period)
        {
            try
            {
                await ReceiveMessage();

                var validityStructure = await _validityPeriodService.GetValidityStructure(collectionYear, period);

                await SendMessage(validityStructure);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }
    }
}