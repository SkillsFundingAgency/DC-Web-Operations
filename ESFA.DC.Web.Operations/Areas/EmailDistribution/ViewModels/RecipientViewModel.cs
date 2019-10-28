using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using ESFA.DC.EmailDistribution.Models;
using ESFA.DC.Web.Operations.Constants;

namespace ESFA.DC.Web.Operations.Areas.EmailDistribution.ViewModels
{
    public class RecipientViewModel : IValidatableObject
    {
        private const string EmailRegEx = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        public string Email { get; set; }

        public List<RecipientGroup> RecipientGroups { get; set; }

        public List<int> SelectedGroupIds { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Email) || !Regex.IsMatch(Email, EmailRegEx))
            {
                yield return new ValidationResult($"Please enter valid email address", new[] { ErrorMessageKeys.Recipient_EmailFieldKey, ErrorMessageKeys.ErrorSummaryKey });
            }

            if (SelectedGroupIds == null || !SelectedGroupIds.Any())
            {
                yield return new ValidationResult($"Please select at least one group", new[] { ErrorMessageKeys.Recipient_GroupsKey, ErrorMessageKeys.ErrorSummaryKey });
            }
        }
    }
}
