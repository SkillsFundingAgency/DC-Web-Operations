using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndILR.Models
{
    public class ValidityPeriodViewModel
    {
        public int Year { get; set; }

        public int Period { get; set; }

        public bool PeriodEndInProgress { get; set; }

        public List<int> AllYears { get; set; }
    }
}
