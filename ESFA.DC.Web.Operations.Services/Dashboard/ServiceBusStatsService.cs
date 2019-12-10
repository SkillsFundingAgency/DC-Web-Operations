using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.Models.Dashboard;
using ESFA.DC.Web.Operations.Models.Dashboard.ServiceBus;
using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.Azure.ServiceBus.Management;

namespace ESFA.DC.Web.Operations.Services.Dashboard
{
    public sealed class ServiceBusStatsService : IServiceBusStatsService
    {
        private readonly ServiceBusSettings _serviceBusSettings;
        private readonly ILogger _logger;

        public ServiceBusStatsService(ServiceBusSettings serviceBusSettings, ILogger logger)
        {
            _serviceBusSettings = serviceBusSettings;
            _logger = logger;
        }

        public async Task<ServiceBusStatusModel> ProvideAsync(CancellationToken cancellationToken)
        {
            List<ServiceBusEntity> queuesList = new List<ServiceBusEntity>();
            List<ServiceBusEntity> topicsList = new List<ServiceBusEntity>();
            List<ServiceBusEntity> topicsIlr = new List<ServiceBusEntity>();

            try
            {
                var mgmtClient = new ManagementClient(_serviceBusSettings.ServiceBusManagementConnectionString);

                var queues = await mgmtClient.GetQueuesAsync(cancellationToken: cancellationToken);

                foreach (QueueDescription queueDescription in queues)
                {
                    if (queueDescription.Path == "crossloadin" || queueDescription.Path == "crossloadout")
                    {
                        continue;
                    }

                    var runtimeInfo =
                        await mgmtClient.GetQueueRuntimeInfoAsync(queueDescription.Path, cancellationToken);

                    queuesList.Add(new ServiceBusEntity
                    {
                        Name = queueDescription.Path.Replace("queue", string.Empty),
                        DeadLetterMessageCount = runtimeInfo.MessageCountDetails.DeadLetterMessageCount,
                        MessageCount = runtimeInfo.MessageCountDetails.ActiveMessageCount,
                        TransferCount = runtimeInfo.MessageCountDetails.TransferMessageCount
                    });
                }

                var topics = await mgmtClient.GetTopicsAsync(cancellationToken: cancellationToken);

                foreach (TopicDescription topicDescription in topics)
                {
                    if (topicDescription.Path == "bundle-1")
                    {
                        continue;
                    }

                    var runtimeInfo =
                        await mgmtClient.GetTopicRuntimeInfoAsync(topicDescription.Path, cancellationToken);

                    var model = new ServiceBusEntity
                    {
                        Name = topicDescription.Path.Replace("submission", string.Empty).Replace("topic", string.Empty),
                        DeadLetterMessageCount = runtimeInfo.MessageCountDetails.DeadLetterMessageCount,
                        MessageCount = runtimeInfo.MessageCountDetails.ActiveMessageCount,
                        TransferCount = runtimeInfo.MessageCountDetails.TransferMessageCount
                    };

                    IList<SubscriptionDescription> subscriptions = await mgmtClient.GetSubscriptionsAsync(topicDescription.Path, cancellationToken: cancellationToken);
                    foreach (SubscriptionDescription subscriptionDescription in subscriptions)
                    {
                        var runtimeInfoSubscription =
                            await mgmtClient.GetSubscriptionRuntimeInfoAsync(runtimeInfo.Path, subscriptionDescription.SubscriptionName, cancellationToken);

                        model.MessageCount += runtimeInfoSubscription.MessageCount;

                        if (model.Name.StartsWith("ilr", StringComparison.OrdinalIgnoreCase))
                        {
                            topicsIlr.Add(new ServiceBusEntity
                            {
                                DeadLetterMessageCount = 0,
                                MessageCount = runtimeInfoSubscription.MessageCount,
                                Name = subscriptionDescription.SubscriptionName.Replace("ReferenceDataRetrieval", "Ref Data").Replace("GenerateFM36", string.Empty),
                                TransferCount = 0
                            });
                        }
                    }

                    topicsList.Add(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get Service Bus stats", ex);
            }

            return new ServiceBusStatusModel
            {
                Queues = queuesList,
                Topics = topicsList,
                Ilr = topicsIlr
            };
        }
    }
}
