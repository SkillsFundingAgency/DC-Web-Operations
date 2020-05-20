using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ESFA.DC.Web.Operations.TagHelpers
{
    [HtmlTargetElement("div", Attributes = HiddenIfNotAuthorisedAttributeName)]
    public class HiddenIfNotAuthorisedTagHelper : TagHelper
    {
        private const string HiddenIfNotAuthorisedAttributeName = "hidden-if-notauthorised";

        [HtmlAttributeName(HiddenIfNotAuthorisedAttributeName)]
        public bool Authorised { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Authorised)
            {
                output.AddClass("hidden", HtmlEncoder.Default);
            }
        }
    }
}
