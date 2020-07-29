using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.FundingClaimsDates
{
    public class FundingClaimsCollectionMetaData
    {
        public int Id { get; set; }

        public int CollectionId { get; set; }

        public int CollectionYear { get; set; }

        public string CollectionName { get; set; }

        public DateTime SubmissionOpenDateUtc { get; set; }

        public DateTime SubmissionCloseDateUtc { get; set; }

        public DateTime? SignatureCloseDateUtc { get; set; }

        public char RequiresSignature { get; set; }

        public DateTime? HelpdeskOpenDateUtc { get; set; }

        public DateTime? DateTimeUpdatedUtc { get; set; }

        public string CreatedBy { get; set; }

        public bool InEditMode { get; set; }

        //public string SubmissionOpenDateUtcFormattedString
        //{
        //    get
        //    {
        //        return string.Concat(SubmissionOpenDateUtc.ToString("d MMMM yyyy", CultureInfo.InvariantCulture), " at ", SubmissionOpenDateUtc.ToString("hh:mm tt", CultureInfo.InvariantCulture).ToLower());
        //    }
        //}
    }
}
