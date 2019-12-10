namespace ESFA.DC.Web.Operations.Models.Dashboard.ServiceBus
{
    public sealed class ServiceBusEntity
    {
        public string Name { get; set; }

        public long MessageCount { get; set; }

        public long DeadLetterMessageCount { get; set; }

        public long TransferCount { get; set; }
    }
}
