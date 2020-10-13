using System.Collections.Generic;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Models.ReferenceData
{
    public class ReferenceDataIndexModel
    {
        public ReferenceDataIndexModel()
        {
            CollectionJobStats = new Dictionary<string, ReferenceDataIndexBase>();
        }

        public Dictionary<string, ReferenceDataIndexBase> CollectionJobStats { get; set; }
    }
}
