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
        private readonly ILogger _logger;
        private readonly IPeriodEndService _periodEndService;
        private readonly PeriodEndHub _periodEndHub;

        private readonly ManualResetEvent timerResetEvent = new ManualResetEvent(false);
        private readonly object timerStopLock = new object();
        private readonly object timerChangeLock = new object();

        private Timer _timer;
        private bool timerStopFlag;

        public TimedHostedService(
            ILogger logger,
            IPeriodEndService periodEndService,
            PeriodEndHub periodEndHub)
        {
            _logger = logger;
            _periodEndService = periodEndService;
            _periodEndHub = periodEndHub;
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
            lock (timerChangeLock)
            {
                timerStopFlag = false;
                timerResetEvent.Set();
                _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5).Milliseconds, Timeout.Infinite);
            }
        }

        private void StopTimer()
        {
            lock (timerChangeLock)
            {
                timerResetEvent.Reset();
                lock (timerStopLock)
                {
                    timerStopFlag = true;
                }

                if (timerResetEvent.WaitOne(5500))
                {
                    _timer?.Dispose();
                    _timer = null;
                }
            }
        }

        private async void DoWork(object state)
        {
            // Get state JSON.
            string result = await _periodEndService.GetPathItemStates();

            // Send JSON to clients.
            await _periodEndHub.SendMessage(result);

            // If we're told to stop then set block to continue, and exit method.
            lock (timerStopLock)
            {
                if (timerStopFlag)
                {
                    timerResetEvent.Set();
                    return;
                }
            }

            // Set timer to tick in 5 seconds.
            _timer.Change(TimeSpan.FromSeconds(5).Milliseconds, Timeout.Infinite);
        }
    }
}