using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ESFA.DC.Web.Operations.TagHelpers
{
    [HtmlTargetElement("a", Attributes = IsEditableReturnPeriodAttribute)]
    public class ReturnPeriodManageLinkTagHelper : TagHelper
    {
        private const string IsEditableReturnPeriodAttribute = "is-editable-rp";
        private const string IsEditableReturnPeriodIdAttribute = "is-editable-rpid";

        [HtmlAttributeName(IsEditableReturnPeriodAttribute)]
        public bool IsEditableReturnPeriodValue { get; set; }

        [HtmlAttributeName(IsEditableReturnPeriodIdAttribute)]
        public int IsEditableReturnPeriodIdValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IsEditableReturnPeriodValue)
            {
                output.Attributes.SetAttribute("href", $"returnperiod/{IsEditableReturnPeriodIdValue}");
                output.RemoveClass("hidden", HtmlEncoder.Default);
            }
        }
    }
}
