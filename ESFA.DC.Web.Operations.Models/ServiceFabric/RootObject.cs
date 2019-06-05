using System.Collections.Generic;
using Microsoft.ServiceFabric.Common;

namespace ESFA.DC.Web.Operations.Models.ServiceFabric
{
    public class RootObject
    {
        public string ContinuationToken { get; set; }

        public List<Item> Items { get; set; }

        public List<NodeInfo> NodeItems { get; set; }
    }
}