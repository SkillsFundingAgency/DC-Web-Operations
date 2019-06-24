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
        private Timer _timer;
        private readonly PeriodEndHub _periodEndHub;

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
            _logger.LogInfo("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfo("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void DoWork(object state)
        {
            var result = _periodEndService.GetPathItemStates().Result;

            _periodEndHub.SendMessage(result);
        }
    }
}