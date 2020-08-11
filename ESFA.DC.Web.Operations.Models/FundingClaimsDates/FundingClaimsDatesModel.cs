using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.FundingClaimsDates
{
    public class FundingClaimsDatesModel
    {
        public List<FundingClaimsCollectionMetaData> FundingClaimsDatesList { get; set; }

        public List<ESFA.DC.CollectionsManagement.Models.Collection> Collections { get; set; }
    }
}
