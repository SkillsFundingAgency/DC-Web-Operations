using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Models.ServiceFabric
{
    public class Item
    {
        public string Name { get; set; }

        public string TypeName { get; set; }

        public string TypeVersion { get; set; }

        public string Status { get; set; }

        public List<Parameter> Parameters { get; set; }

        public string HealthState { get; set; }

        public string ApplicationDefinitionKind { get; set; }

        public string Id { get; set; }
    }
}