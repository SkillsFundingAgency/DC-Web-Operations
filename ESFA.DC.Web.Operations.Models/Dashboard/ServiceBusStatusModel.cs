using System.Collections.Generic;
using ESFA.DC.Web.Operations.Models.Dashboard.ServiceBus;

namespace ESFA.DC.Web.Operations.Models.Dashboard
{
    public sealed class ServiceBusStatusModel
    {
        public IEnumerable<ServiceBusEntity> Queues { get; set; }

        public IEnumerable<ServiceBusEntity> Topics { get; set; }

        public IEnumerable<ServiceBusEntity> Ilr { get; set; }
    }
}