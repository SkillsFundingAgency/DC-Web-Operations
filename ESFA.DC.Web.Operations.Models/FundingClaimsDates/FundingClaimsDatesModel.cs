using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.FundingClaimsDates
{
    public class FundingClaimsDatesModel
    {
        public List<FundingClaimsCollectionMetaData> FundingClaimsCollectionMetaDataList { get; set; }

        public List<int> CollectionYears
        {
            get { return FundingClaimsCollectionMetaDataList.Select(x => x.CollectionYear).Distinct().ToList(); }
        }
    }
}
