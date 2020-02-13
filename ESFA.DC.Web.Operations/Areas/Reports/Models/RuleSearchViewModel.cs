using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Areas.Reports.Models
{
    public class RuleSearchViewModel
    {
        public IEnumerable<int> CollectionYears { get; set; }

        public IEnumerable<string> Rules { get; set; }

        public int Year { get; set; }
    }
}
