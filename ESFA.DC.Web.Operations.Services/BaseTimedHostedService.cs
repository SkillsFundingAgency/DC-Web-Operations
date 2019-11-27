using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Services.Event;
using Microsoft.Extensions.Hosting;

namespace ESFA.DC.Web.Operations.Services
{
    public abstract class BaseTimedHostedService : IHostedService, IDisposable
    {
        private const int TimerCadenceMs = 3000;
        private const int SleepAfterMinutes = 5;

        private readonly string _logPrefix;
        private readonly ILogger _logger;
        private readonly ManualResetEvent _timerResetEvent = new ManualResetEvent(false);
        private readonly object _timerStopLock = new object();
        private readonly object _timerChangeLock = new object();
        private volatile bool _timerStopFlag;
        private Timer _timer;
        private CancellationTokenSource _cancellationTokenSource;
        private DateTime _lastClientTimestamp = DateTime.MinValue;

        protected BaseTimedHostedService(string logPrefix, ILogger logger)
        {
            _logPrefix = logPrefix;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfo($"{_logPrefix} Timed SignalR Background Service is starting 1");
            lock (_timerStopLock)
            {
                if (_timerStopFlag)
                {
                    return Task.CompletedTask;
                }

                _logger.LogInfo($"{_logPrefix} Timed SignalR Background Service is starting 2");
                StartTimer();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfo($"{_logPrefix} Timed SignalR Background Service is stopping");
            StopTimer();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            StopTimer();
        }

        protected abstract void DoWork(CancellationToken cancellationToken);

        protected void RegisterClient(object sender, EventArgs eventArgs)
        {
            SignalrEventArgs signalrEventArgs = eventArgs as SignalrEventArgs;
            _logger.LogInfo($"{_logPrefix} Timed SignalR Background Service heard from client {signalrEventArgs?.ConnectionId ?? "Unknown"}");
            _lastClientTimestamp = DateTime.UtcNow;
            lock (_timerStopLock)
            {
                if (!_timerStopFlag)
                {
                    return;
                }

                _logger.LogInfo($"{_logPrefix} Timed SignalR Background Service heard from a client {signalrEventArgs?.ConnectionId ?? "Unknown"}, starting timer");
                StartTimer();
            }
        }

        /// <summary>
        /// This method is always called within _timerStopLock.
        /// </summary>
        private void StartTimer()
        {
            lock (_timerChangeLock)
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                _timerStopFlag = false;
                _timerResetEvent.Set();
                _timer = new Timer(TimerTick, null, TimerCadenceMs, Timeout.Infinite);
            }
        }

        private void StopTimer(bool block = true)
        {
            lock (_timerChangeLock)
            {
                // Stop and Dispose called together, prevent running method twice.
                if (_timer == null)
                {
                    _logger.LogInfo($"{_logPrefix} Timed SignalR Background Service was stopping and disposing, not continuing");
                    return;
                }

                lock (_timerStopLock)
                {
                    // Make sure we aren't trying to stop because we haven't heard from a client, but then a client message has come in
                    if (!block && !NoActiveClients())
                    {
                        _logger.LogInfo($"{_logPrefix} Timed SignalR Background Service was stopping, but a listener was detected, so continuing");
                        return;
                    }

                    _timerStopFlag = true;
                }

                _cancellationTokenSource.Cancel();
                _timerResetEvent.Reset();

                if (block && !_timerResetEvent.WaitOne(TimerCadenceMs * 2))
                {
                    _logger.LogWarning($"{_logPrefix} Timed SignalR Background Service did not terminate correctly");
                }

                _timer?.Dispose();
                _timer = null;
            }
        }

        private void TimerTick(object state)
        {
            // If we're told to stop then set block to continue, and exit method.
            lock (_timerStopLock)
            {
                if (_timerStopFlag)
                {
                    _logger.LogInfo($"{_logPrefix} Timed SignalR Background Service blocking in TimerTick");
                    _timerResetEvent.Set();
                    return;
                }
            }

            // We haven't received a ping from any clients, so sleep this timer.
            if (NoActiveClients())
            {
                _logger.LogInfo($"{_logPrefix} Timed SignalR Background Service sleeping in TimerTick as no one listening");
                StopTimer(false);
                return;
            }

            try
            {
                DoWork(_cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_logPrefix} Timed SignalR Background Service failed to DoWork", ex);
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
