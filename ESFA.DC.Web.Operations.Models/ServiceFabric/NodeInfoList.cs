using Microsoft.ServiceFabric.Common;

namespace ESFA.DC.Web.Operations.Models.ServiceFabric
{
    public class NodeInfoList
    {
        public string ContinuationToken { get; set; }

        public NodeInfo[] Items { get; set; }
    }
}