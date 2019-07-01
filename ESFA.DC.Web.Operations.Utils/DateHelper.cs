using System;

namespace ESFA.DC.Web.Operations.Utils
{
    public class DateHelper
    {
        public static string GetUrlFriendlyDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        }
    }
}