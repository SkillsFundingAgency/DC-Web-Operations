﻿using System;
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

        public void RegisterClient(object sender, EventArgs a)
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

            if (_lastClientTimestamp < DateTime.UtcNow.AddMinutes(-SleepAfterMinutes))
            {
                StopTimer();
            }
        }
    }
}