using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ESFA.DC.Web.Operations.TagHelpers
{
    [HtmlTargetElement("input", Attributes = IsMcaDisabledAttributeName)]
    public class IsMcaCheckboxDisabledTagHelper : TagHelper
    {
        private const string IsMcaDisabledAttributeName = "is-ismca-disabled";

        [HtmlAttributeName(IsMcaDisabledAttributeName)]
        public string Ukprn { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(Ukprn))
            {
                output.Attributes.SetAttribute("disabled", "true");
            }
        }
    }
}
