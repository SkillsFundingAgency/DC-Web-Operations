using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Enums
{
    public enum FileNameValidationResult
    {
        Valid = 1,
        EmptyFile = 2,
        InvalidFileNameFormat = 3,
        InvalidFileExtension = 4
    }
}
