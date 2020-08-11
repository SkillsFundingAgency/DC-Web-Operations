using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Web.Operations.Models.FundingClaimsDates;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Models
{
    public class FundingClaimsDatesViewModel
    {
        public IEnumerable<FundingClaimsCollectionMetaData> FundingClaimsDatesList { get; set; }

        public IEnumerable<int> CollectionYears { get; set; }

        public int CollectionYearSelected { get; set; }

        public List<SelectListItem> Years => CollectionYears
            .Select(n => new SelectListItem { Text = n.ToString(CultureInfo.CurrentCulture), Value = n.ToString(CultureInfo.CurrentCulture) })
            .OrderByDescending(o => o.Value)
            .ToList();

        public List<Collection> Collections { get; set; }
    }
}
