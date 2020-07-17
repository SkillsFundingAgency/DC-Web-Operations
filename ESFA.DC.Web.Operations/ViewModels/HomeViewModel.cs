using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ESFA.DC.Web.Operations.ViewModels
{
    public class HomeViewModel
    {
        public string DashboardStats { get; set; }

        public int CollectionYear { get; set; }

        public List<int> CollectionYears { get; set; }

        public List<SelectListItem> Years => CollectionYears
            .Select(n => new SelectListItem { Text = n.ToString(CultureInfo.CurrentCulture), Value = n.ToString(CultureInfo.CurrentCulture) })
            .OrderByDescending(o => o.Text)
            .ToList();
    }
}
