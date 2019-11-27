using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.Models.Dashboard;
using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.Azure.ServiceBus.Management;

namespace ESFA.DC.Web.Operations.Services.Dashboard
{
    public sealed class ServiceBusStatsService : IServiceBusStatsService
    {
        private readonly ServiceBusSettings _serviceBusSettings;

        public ServiceBusStatsService(ServiceBusSettings serviceBusSettings)
        {
            _serviceBusSettings = serviceBusSettings;
        }

        public async Task<IEnumerable<ServiceBusStatusModel>> ProvideAsync(CancellationToken cancellationToken)
        {
            List<ServiceBusStatusModel> models = new List<ServiceBusStatusModel>();

            var mgmtClient = new ManagementClient(_serviceBusSettings.ServiceBusManagementConnectionString);

            var queues = await mgmtClient.GetQueuesAsync(cancellationToken: cancellationToken);

            foreach (QueueDescription queueDescription in queues)
            {
                var runtimeInfo = await mgmtClient.GetQueueRuntimeInfoAsync(queueDescription.Path, cancellationToken);
                models.Add(new ServiceBusStatusModel
                {
                    Name = queueDescription.Path,
                    DeadLetterMessageCount = runtimeInfo.MessageCountDetails.DeadLetterMessageCount,
                    MessageCount = runtimeInfo.MessageCountDetails.ActiveMessageCount,
                    TransferCount = runtimeInfo.MessageCountDetails.TransferMessageCount
                });
            }

            return models;
        }
    }
}
