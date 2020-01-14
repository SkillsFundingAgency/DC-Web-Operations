using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.Web.Operations.Models.Enums;

namespace ESFA.DC.Web.Operations.Models
{
    public class FileNameValidationResultModel
    {
        public FileNameValidationResult ValidationResult { get; set; }

        public string SummaryError { get; set; }

        public string FieldError { get; set; }
    }
}
