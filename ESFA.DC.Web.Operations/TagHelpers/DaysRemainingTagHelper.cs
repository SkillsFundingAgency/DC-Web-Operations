using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ESFA.DC.Web.Operations.TagHelpers
{
    [HtmlTargetElement("span", Attributes = DaysRemainingAttribute)]
    public class DaysRemainingTagHelper : TagHelper
    {
        private const string DaysRemainingAttribute = "days-remaining";

        [HtmlAttributeName(DaysRemainingAttribute)]
        public int? DaysRemainingAttributeValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (DaysRemainingAttributeValue.HasValue)
            {
                output.Content.SetContent($"{DaysRemainingAttributeValue.Value} day/s remaining");
            }
        }
    }
}
