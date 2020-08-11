using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ESFA.DC.Web.Operations.Utils.Extensions;

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

        public DateTime HelpdeskOpenDateUtc { get; set; }

        public DateTime DateTimeUpdatedUtc { get; set; }

        public string UpdatedBy { get; set; }

        public bool InEditMode { get; set; }

        public string SubmissionOpenDateUtcFormattedString
        {
            get { return SubmissionOpenDateUtc.ToDateTimeString(); }
        }

        public string SubmissionCloseDateUtcFormattedString
        {
            get { return SubmissionCloseDateUtc.ToDateTimeString(); }
        }

        public string HelpdeskOpenDateUtcFormattedString
        {
            get { return HelpdeskOpenDateUtc.ToDateTimeString(); }
        }

        public string SignatureCloseDateUtcFormattedString
        {
            get { return SignatureCloseDateUtc?.ToDateTimeString(); }
        }
    }
}
