﻿using System;
using System.Globalization;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Models
{
    public class ReferenceDataIndexBase
    {
        public DateTime LastUpdatedDateTime { get; set; }

        public string LastUpdatedByWho { get; set; }

        public string LastUpdatedDateTimeFormattedDisplay =>
            $"{LastUpdatedDateTime.ToString("d MMMM yyyy", CultureInfo.InvariantCulture)} at {LastUpdatedDateTime.ToString("h:mm tt", CultureInfo.InvariantCulture).ToLower(CultureInfo.CurrentCulture)}";
    }
}