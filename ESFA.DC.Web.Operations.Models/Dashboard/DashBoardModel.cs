using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Models.Dashboard
{
    public sealed class DashBoardModel
    {
        public IEnumerable<ServiceBusStatusModel> ServiceBusStats { get; set; }

        public JobStatsModel JobStats { get; set; }
    }
}
