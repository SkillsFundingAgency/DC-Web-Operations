using System;
using System.Globalization;

namespace ESFA.DC.Web.Operations.Models.ReferenceData
{
    public class ReferenceDataIndexBase
    {
        public DateTime LastUpdatedDateTime { get; set; }

        public string LastUpdatedByWho { get; set; }

        public string LastUpdatedDateTimeFormattedDisplay => LastUpdatedDateTime.Year != 1 ? $"{LastUpdatedDateTime.ToString("d MMMM yyyy", CultureInfo.InvariantCulture)} at {LastUpdatedDateTime.ToString("h:mm tt", CultureInfo.InvariantCulture).ToLower(CultureInfo.CurrentCulture)}" : "Data unavailable";

        public bool Valid { get; set; }
    }
}