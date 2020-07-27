using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToString("dd-MM-yyyy");
        }

        public static string ToTime(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm");
        }
    }
}
