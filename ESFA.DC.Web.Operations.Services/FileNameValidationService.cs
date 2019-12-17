using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Enums;

namespace ESFA.DC.Web.Operations.Services
{
    public class FileNameValidationService : IFileNameValidationService
    {
        protected IEnumerable<string> FileNameExtensions => new List<string>() { ".XLSX" };

        public async Task<FileNameValidationResultModel> ValidateFileNameAsync(string collectionName, string filenameRegex, string fileName, long? fileSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = ValidateEmptyFile(fileName, fileSize);
            if (result != null)
            {
                return result;
            }

            string ext = Path.GetExtension(fileName);
            result = ValidateExtension(ext, "Your file must be in an XML or Zip format");
            if (result != null)
            {
                return result;
            }

            result = ValidateRegex(filenameRegex, fileName, $"File name should use the format PROVIDERS-YYYY-yyyymmdd-hhmmss.XLSX");
            if (result != null)
            {
                return result;
            }

            return new FileNameValidationResultModel()
            {
                ValidationResult = FileNameValidationResult.Valid
            };
        }

        public FileNameValidationResultModel ValidateEmptyFile(string fileName, long? fileSize)
        {
            if (string.IsNullOrEmpty(fileName) || fileSize == null || fileSize.Value == 0)
            {
                return new FileNameValidationResultModel()
                {
                    ValidationResult = FileNameValidationResult.EmptyFile,
                    FieldError = "Choose a file to upload",
                    SummaryError = "Check file you want to upload"
                };
            }

            return null;
        }

        public FileNameValidationResultModel ValidateExtension(string extension, string errorMessage)
        {
            if (!FileNameExtensions.Contains(extension.ToUpperInvariant()))
            {
                return new FileNameValidationResultModel()
                {
                    ValidationResult = FileNameValidationResult.InvalidFileExtension,
                    FieldError = errorMessage,
                    SummaryError = errorMessage
                };
            }

            return null;
        }

        public FileNameValidationResultModel ValidateRegex(string filenameRegex, string fileName, string errorMessage)
        {
            if (!Regex.IsMatch(fileName, filenameRegex))
            {
                return new FileNameValidationResultModel()
                {
                    ValidationResult = FileNameValidationResult.InvalidFileExtension,
                    FieldError = errorMessage,
                    SummaryError = errorMessage
                };
            }

            return null;
        }
    }
}
