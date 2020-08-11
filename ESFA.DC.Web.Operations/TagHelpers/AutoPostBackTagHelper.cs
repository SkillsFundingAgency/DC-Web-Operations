using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ESFA.DC.Web.Operations.TagHelpers
{
    [HtmlTargetElement("select", Attributes = "autopostback")]
    public class AutoPostBackTagHelper : TagHelper
    {
        public bool autopostback { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (autopostback)
            {
                output.Attributes.SetAttribute("onchange", "this.form.submit();");
            }
        }
    }
}
