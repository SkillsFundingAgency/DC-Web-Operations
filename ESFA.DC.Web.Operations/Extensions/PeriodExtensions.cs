using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Extensions
{
    public static class PeriodExtensions
    {
        public static string ToPeriodName(this int periodNumber)
        {
            return $"R{periodNumber.ToString("D2")}";
        }
    }
}
