using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.FundingClaimsDates
{
    public class FundingClaimsCollectionMetaDataLastUpdate
    {
        public int Id { get; set; }

        public DateTime? DateTimeUpdatedUtc { get; set; }

        public string CreatedBy { get; set; }
    }
}
