using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
            if (!IsValidEmail(Email))
            {
                yield return new ValidationResult($"Please enter valid email address", new[] { ErrorMessageKeys.Recipient_EmailFieldKey, ErrorMessageKeys.ErrorSummaryKey });
            }

            if (SelectedGroupIds == null || !SelectedGroupIds.Any())
            {
                yield return new ValidationResult($"Please select at least one group", new[] { ErrorMessageKeys.Recipient_GroupsKey, ErrorMessageKeys.ErrorSummaryKey });
            }
        }

        // Reference: https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format?redirectedfrom=MSDN
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(
                    email,
                    EmailRegEx,
                    RegexOptions.IgnoreCase,
                    TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
