using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ESFA.DC.Web.Operations.TagHelpers
{
    [HtmlTargetElement("a", Attributes = GovUkRequestPathSearchText)]
    public class GovukNavigationClassTagHelper : TagHelper
    {
        private const string GovUkRequestPathSearchText = "govuk-request-path-search-text";
        private const string GovUkHeadingSmallClass = "govuk-heading-s ";
        private readonly IHttpContextAccessor _contextAccessor;

        public GovukNavigationClassTagHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        [HtmlAttributeName(GovUkRequestPathSearchText)]
        public string GovUkRequestPathSearchValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (_contextAccessor.HttpContext.Request.Path.HasValue && _contextAccessor.HttpContext.Request.Path.Value.Contains(GovUkRequestPathSearchValue))
            {
                output.Attributes.SetAttribute("class", GovUkHeadingSmallClass + "active");
            }
        }
    }
}
