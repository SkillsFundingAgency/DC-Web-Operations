using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ESFA.DC.Web.Operations.Utils.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToDateTimeString(this DateTime dateTime)
        {
            return string.Concat(dateTime.ToString("d MMMM yyyy", CultureInfo.InvariantCulture), " at ", dateTime.ToString("hh:mm tt", CultureInfo.InvariantCulture).ToLower());
        }
    }
}
