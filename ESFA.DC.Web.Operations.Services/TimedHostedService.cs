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

        private readonly IPeriodService _periodService;
        private readonly ILogger _logger;
        private readonly IPeriodEndService _periodEndService;
        private readonly PeriodEndHub _periodEndHub;
        private readonly PeriodEndPrepHub _periodEndPrepHub;

        private readonly ManualResetEvent _timerResetEvent = new ManualResetEvent(false);
        private readonly object _timerStopLock = new object();
        private readonly object _timerChangeLock = new object();

        private Timer _timer;
        private bool _timerStopFlag;
        private CancellationTokenSource _cancellationTokenSource;

        public TimedHostedService(
            IPeriodService periodService,
            ILogger logger,
            IPeriodEndService periodEndService,
            PeriodEndHub periodEndHub,
            PeriodEndPrepHub periodEndPrepHub)
        {
            _periodService = periodService;
            _logger = logger;
            _periodEndService = periodEndService;
            _periodEndHub = periodEndHub;
            _periodEndPrepHub = periodEndPrepHub;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfo("Timed SignalR Background Service is starting.");

            StartTimer();

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

        private void StopTimer()
        {
            lock (_timerChangeLock)
            {
                // Stop and Dispose called together, prevent running method twice.
                if (_timer == null)
                {
                    return;
                }

                _cancellationTokenSource.Cancel();
                _timerResetEvent.Reset();
                lock (_timerStopLock)
                {
                    _timerStopFlag = true;
                }

                if (!_timerResetEvent.WaitOne(TimerCadenceMs * 2))
                {
                    _logger.LogWarning("Timed SignalR Background Service did not terminate correctly");
                }

                _timer?.Dispose();
                _timer = null;
            }
        }

        private async void DoWork(object state)
        {
            var currentPeriod = await _periodService.ReturnPeriod(DateTime.UtcNow);

            try
            {
                // Get state JSON.
                string result = await _periodEndService.GetPathItemStates(currentPeriod.Year, currentPeriod.Period, _cancellationTokenSource.Token);

                // Send JSON to clients.
                await _periodEndHub.SendMessage(result, _cancellationTokenSource.Token);

                await _periodEndPrepHub.SendMessage(string.Empty, _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Timed SignalR Background Service failed to DoWork", ex);
            }

            // If we're told to stop then set block to continue, and exit method.
            lock (_timerStopLock)
            {
                if (_timerStopFlag)
                {
                    _timerResetEvent.Set();
                    return;
                }
            }

            // Set timer to tick in TimerCadenceMs milliseconds.
            _timer.Change(TimerCadenceMs, Timeout.Infinite);
        }
    }
}