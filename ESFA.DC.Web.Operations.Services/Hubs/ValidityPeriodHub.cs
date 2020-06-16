using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class ValidityPeriodHub : BaseHub<ValidityPeriodHub>
    {
        private readonly ILogger _logger;
        private readonly IValidityPeriodService _validityPeriodService;

        public ValidityPeriodHub(
            IHubEventBase eventBase,
            IHubContext<ValidityPeriodHub> hubContext,
            ILogger logger,
            IValidityPeriodService validityPeriodService)
            : base(eventBase, hubContext, logger)
        {
            _logger = logger;
            _validityPeriodService = validityPeriodService;
        }

        public async Task GetValidityPeriodList(int period)
        {
            try
            {
                await ReceiveMessage();

                var validityPeriodData = await _validityPeriodService.GetValidityPeriodList(period);

                await SendMessage(validityPeriodData);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task UpdateValidityPeriod(int period, object validityPeriods)
        {
            try
            {
                await ReceiveMessage();

                var updatedDataCount = await _validityPeriodService.UpdateValidityPeriod(period, validityPeriods);

                var validityPeriodData = await _validityPeriodService.GetValidityPeriodList(period);

                await SendMessage(validityPeriodData);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }
    }
}