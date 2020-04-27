using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Models
{
    public sealed class PeriodEndViewModel
    {
      public PathYearPeriod ILRPeriodEndInfo { get; set; }

      public PathYearPeriod NCSPeriodEndInfo { get; set; }

      public PathYearPeriod ALLFPeriodEndInfo { get; set; }
    }
}
