using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Models
{
    public sealed class PeriodEndViewModel
    {
        public int Period { get; set; }

        public int Year { get; set; }

        public bool IsCurrentPeriod { get; set; }

        public bool Closed { get; set; }
    }
}
