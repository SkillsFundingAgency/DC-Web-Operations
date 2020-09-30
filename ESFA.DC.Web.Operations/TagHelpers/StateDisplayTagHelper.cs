using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ESFA.DC.Web.Operations.TagHelpers
{
    public class StateDisplayTagHelper : TagHelper
    {
        [HtmlAttributeName("include-slow-connection")]
        public bool IncludeSlowConnection { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output), "output cannot be null");
            }

            if (IncludeSlowConnection)
            {
                output.Content.AppendHtml("<label id='slowConnection' alt='Connection is slow at the moment'></label>");
            }

            output.Content.AppendHtml("<label id='lastSync'></label>");
            output.Content.AppendHtml("<label id='state'></label>");
        }
    }
}
