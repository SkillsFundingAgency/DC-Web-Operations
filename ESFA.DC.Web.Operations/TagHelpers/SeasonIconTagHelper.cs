using ESFA.DC.DateTimeProvider.Interface;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ESFA.DC.Web.Operations.TagHelpers
{
    [HtmlTargetElement("season")]
    public class SeasonIconTagHelper : TagHelper
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public SeasonIconTagHelper(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var nowUtc = _dateTimeProvider.GetNowUtc();

            switch (nowUtc.Month)
            {
                case 1:
                    output.Content.SetContent("🍾");
                    break;
                case 2:
                    output.Content.SetContent("💗");
                    break;
                case 3:
                    output.Content.SetContent("🍀");
                    break;
                case 4:
                    output.Content.SetContent("🐰");
                    break;
                case 5:
                    output.Content.SetContent("💃");
                    break;
                case 6:
                    output.Content.SetContent("🌞");
                    break;
                case 7:
                    output.Content.SetContent("🌴");
                    break;
                case 8:
                    output.Content.SetContent("🍦");
                    break;
                case 9:
                    output.Content.SetContent("📅");
                    break;
                case 10:
                    output.Content.SetContent("🎃");
                    break;
                case 11:
                    output.Content.SetContent("🛒");
                    break;
                case 12:
                    output.Content.SetContent("🎄");
                    break;
            }
        }
    }
}
