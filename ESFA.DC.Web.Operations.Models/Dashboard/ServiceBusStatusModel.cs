namespace ESFA.DC.Web.Operations.Models.Dashboard
{
    public sealed class ServiceBusStatusModel
    {
        public string Name { get; set; }

        public long MessageCount { get; set; }

        public long DeadLetterMessageCount { get; set; }

        public long TransferCount { get; set; }
    }
}
