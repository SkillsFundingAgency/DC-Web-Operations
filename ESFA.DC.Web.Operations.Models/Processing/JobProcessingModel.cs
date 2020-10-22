using System.Collections.Generic;
using ESFA.DC.Jobs.Model.Processing;

namespace ESFA.DC.Web.Operations.Models.Processing
{
    public class JobProcessingModel<TLookupModel>
        where TLookupModel : ProcessingLookupModelBase
    {
        public JobProcessingModel()
        {
            Jobs = new Dictionary<int, List<TLookupModel>>();
        }

        public int? CollectionYear { get; set; }

        public Dictionary<int, List<TLookupModel>> Jobs { get; set; }

        public string JobProcessingType { get; set; }
    }
}
