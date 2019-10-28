using ESFA.DC.Web.Operations.Areas.EmailDistribution.ViewModels;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Web.Operations.Tests.EmailDistribution
{
    public class AddRecipientValidationTests
    {
        [Theory]
        [InlineData("test@test", false)]
        [InlineData("test.com.uk", false)]
        [InlineData("js@abc..com", false)]
        [InlineData("abcd@abc.com", true)]
        [InlineData("d.j@server1.company.com", true)]
        public void EmailValidationTest(string email, bool expectation)
        {
            RecipientViewModel model = new RecipientViewModel();

            model.IsValidEmail(email).Should().Be(expectation);
        }
    }
}
