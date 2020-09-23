using ESFA.DC.Web.Operations.Services.FileValidation.StandardValidator;
using Xunit;

namespace ESFA.DC.Web.Operations.Services.Tests
{
    public class StandardFileNameValidationServiceTests
    {
        [Fact]
        public void GetFileDateTime_CorrectlyParsesDateFromFileName()
        {
            // Arrange
            var sut = new StandardFileNameValidationService(null, null, null, null, null, null);

            // Act
            var result = sut.GetFileDateTime(null, "MCAGLA_FullName_RD-202009101512.csv");

            // Assert
            Assert.Equal(result, new System.DateTime(2020, 09, 10, 15, 12, 0));
        }
    }
}
