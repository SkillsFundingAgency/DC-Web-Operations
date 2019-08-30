using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Services.Hubs;
using Microsoft.Extensions.Hosting;

namespace ESFA.DC.Web.Operations.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private const int TimerCadenceMs = 5000;
        private const int SleepAfterMinutes = 5;
        private readonly IPeriodService _periodService;
        private readonly ILogger _logger;
        private readonly IPeriodEndService _periodEndService;
        private readonly PeriodEndHub _periodEndHub;
        private readonly PeriodEndPrepHub _periodEndPrepHub;
        private readonly ManualResetEvent _timerResetEvent = new ManualResetEvent(false);
        private readonly object _timerStopLock = new object();
        private readonly object _timerChangeLock = new object();
        private Timer _timer;
        private volatile bool _timerStopFlag;
        private CancellationTokenSource _cancellationTokenSource;
        private DateTime _lastClientTimestamp = DateTime.MinValue;

        public TimedHostedService(
            IPeriodService periodService,
            ILogger logger,
            IPeriodEndService periodEndService,
            IHubEventBase eventBase,
            PeriodEndHub periodEndHub,
            PeriodEndPrepHub periodEndPrepHub)
        {
            _periodService = periodService;
            _logger = logger;
            _periodEndService = periodEndService;
            _periodEndHub = periodEndHub;
            _periodEndPrepHub = periodEndPrepHub;
            eventBase.PeriodEndHubCallback += RegisterClient;
            eventBase.PeriodEndHubPrepCallback += RegisterClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfo("Timed SignalR Background Service is starting.");
            lock (_timerStopLock)
            {
                if (!_timerStopFlag)
                {
                    return Task.CompletedTask;
                }

                StartTimer();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfo("Timed SignalR Background Service is stopping.");
            StopTimer();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            StopTimer();
        }

        private void RegisterClient(object sender, EventArgs a)
        {
            _lastClientTimestamp = DateTime.UtcNow;
            lock (_timerStopLock)
            {
                if (!_timerStopFlag)
                {
                    return;
                }

                StartTimer();
            }
        }

        private void StartTimer()
        {
            lock (_timerChangeLock)
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                _timerStopFlag = false;
                _timerResetEvent.Set();
                _timer = new Timer(DoWork, null, TimerCadenceMs, Timeout.Infinite);
            }
        }

        private void StopTimer(bool block = true)
        {
            lock (_timerChangeLock)
            {
                // Stop and Dispose called together, prevent running method twice.
                if (_timer == null)
                {
                    return;
                }

                lock (_timerStopLock)
                {
                    // Make sure we aren't trying to stop because we haven't heard from a client, but then a client message has come in
                    if (!block && !NoActiveClients())
                    {
                        return;
                    }

                    _timerStopFlag = true;
                }

                _cancellationTokenSource.Cancel();
                _timerResetEvent.Reset();

                if (block && !_timerResetEvent.WaitOne(TimerCadenceMs * 2))
                {
                    _logger.LogWarning("Timed SignalR Background Service did not terminate correctly");
                }

                _timer?.Dispose();
                _timer = null;
            }
        }

        private async void DoWork(object state)
        {
            // If we're told to stop then set block to continue, and exit method.
            lock (_timerStopLock)
            {
                if (_timerStopFlag)
                {
                    _timerResetEvent.Set();
                    return;
                }
            }

            // We haven't received a ping from any clients, so sleep this timer.
            if (NoActiveClients())
            {
                StopTimer(false);
                return;
            }

            try
            {
                var currentPeriod = await _periodService.ReturnPeriod();

                // Get state JSON.
                string pathItemStates = await _periodEndService.GetPathItemStates(currentPeriod.Year, currentPeriod.Period, _cancellationTokenSource.Token);
                string failedJobs = await _periodEndService.GetFailedJobs(
                    "ILR",
                    currentPeriod.Year,
                    currentPeriod.Period,
                    _cancellationTokenSource.Token);
                string referenceDataJobs = await _periodEndService.GetReferenceDataJobs(_cancellationTokenSource.Token);

                // Send JSON to clients.
                await _periodEndHub.SendMessage(pathItemStates, _cancellationTokenSource.Token);
                await _periodEndPrepHub.SendMessage(referenceDataJobs, failedJobs, _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Timed SignalR Background Service failed to DoWork", ex);
            }

            // Set timer to tick in TimerCadenceMs milliseconds.
            _timer.Change(TimerCadenceMs, Timeout.Infinite);
        }

        private bool NoActiveClients()
        {
            return _lastClientTimestamp < DateTime.UtcNow.AddMinutes(-SleepAfterMinutes);
        }
    }
}