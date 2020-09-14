using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs.PeriodEnd.ILR
{
    public class PeriodEndHub : Hub
    {
        private const string CollectionType = CollectionTypes.ILR;
        private readonly IHubEventBase _eventBase;
        private readonly IHubContext<PeriodEndHub> _hubContext;
        private readonly IPeriodEndService _periodEndService;
        private readonly IEmailService _emailService;
        private readonly IStateService _stateService;
        private readonly IPeriodService _periodService;
        private readonly IApiAvailabilityService _apiAvailabilityService;
        private readonly ILogger _logger;

        public PeriodEndHub(
            IHubEventBase eventBase,
            IHubContext<PeriodEndHub> hubContext,
            IPeriodEndService periodEndService,
            IEmailService emailService,
            IStateService stateService,
            IPeriodService periodService,
            IApiAvailabilityService apiAvailabilityService,
            ILogger logger)
        {
            _eventBase = eventBase;
            _hubContext = hubContext;
            _periodEndService = periodEndService;
            _emailService = emailService;
            _stateService = stateService;
            _periodService = periodService;
            _apiAvailabilityService = apiAvailabilityService;
            _logger = logger;
        }

        public async Task SendMessage(string paths, CancellationToken cancellationToken)
        {
            await SetButtonStates(cancellationToken);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", paths, cancellationToken);
        }

        public async Task ReceiveMessage()
        {
            try
            {
                _eventBase.TriggerHub(Context.ConnectionId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task StartPeriodEnd(int collectionYear, int period)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("DisableStartPeriodEnd");
                await _periodEndService.StartPeriodEndAsync(collectionYear, period, CollectionType);

                await _emailService.SendEmail(EmailIds.PeriodEndStartedEmail, period, Constants.IlrPeriodPrefix);

                await _apiAvailabilityService.SetApiAvailabilityAsync(ApiNameConstants.Learner, ApiUpdateProcessConstants.PE,  false, CancellationToken.None);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task UnPauseReferenceJobs(int collectionYear, int period)
        {
            try
            {
                await _periodEndService.ToggleReferenceDataJobsAsync(collectionYear, period, false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task PublishProviderReports(int collectionYear, int period)
        {
            try
            {
                await _periodEndService.PublishProviderReportsAsync(collectionYear, period, CollectionType);
                await _emailService.SendEmail(EmailIds.ReportsPublishedEmail, period, Constants.IlrPeriodPrefix);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task PublishMcaReports(int collectionYear, int period)
        {
            try
            {
                await _periodEndService.PublishMcaReportsAsync(collectionYear, period, CollectionType);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task ClosePeriodEnd(int collectionYear, int period)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("TurnOffMessage");
                var paused = await _periodEndService.ClosePeriodEndAsync(collectionYear, period, CollectionType);

                if (paused)
                {
                    await _hubContext.Clients.All.SendAsync("ReferenceJobsButtonState");
                }

                await _apiAvailabilityService.SetApiAvailabilityAsync(ApiNameConstants.Learner, ApiUpdateProcessConstants.PE,  true, CancellationToken.None);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task Proceed(int collectionYear, int period, int pathId, int pathItemId)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("DisablePathItemProceed", pathItemId);
                await _periodEndService.ProceedAsync(collectionYear, period, pathId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task ReSubmitJob(int jobId)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("DisableJobReSubmit", jobId);
                await _periodEndService.ReSubmitFailedJobAsync(jobId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        private async Task SetButtonStates(CancellationToken cancellationToken)
        {
            var period = await _periodService.ReturnPeriod(CollectionType, cancellationToken);
            var periodClosed = period.PeriodClosed;

            string stateString = await _periodEndService.GetPathItemStatesAsync(period.Year.Value, period.Period, CollectionType, cancellationToken);
            var state = _stateService.GetMainState(stateString);

            var startEnabled = periodClosed && !state.PeriodEndStarted;
            if (PeriodEndState.CurrentAction != Constants.Action_StartPeriodEndButton)
            {
                await _hubContext.Clients.All.SendAsync(Constants.Action_StartPeriodEndButton, startEnabled, cancellationToken);
            }

            if (PeriodEndState.CurrentAction != Constants.Action_MCAReportsButton)
            {
                var mcaEnabled = periodClosed && !startEnabled && !state.McaReportsPublished && state.McaReportsReady;
                await _hubContext.Clients.All.SendAsync(Constants.Action_MCAReportsButton, mcaEnabled, cancellationToken);
            }

            if (PeriodEndState.CurrentAction != Constants.Action_ProviderReportsButton)
            {
                var providerEnabled = periodClosed && !state.ProviderReportsPublished && state.ProviderReportsReady;
                await _hubContext.Clients.All.SendAsync(Constants.Action_ProviderReportsButton, providerEnabled, cancellationToken);
            }

            if (PeriodEndState.CurrentAction != Constants.Action_PeriodClosedButton)
            {
                var closeEnabled = periodClosed && !state.PeriodEndFinished && state.McaReportsPublished && state.ProviderReportsPublished;
                await _hubContext.Clients.All.SendAsync(Constants.Action_PeriodClosedButton, closeEnabled, cancellationToken);
            }
        }
    }
}