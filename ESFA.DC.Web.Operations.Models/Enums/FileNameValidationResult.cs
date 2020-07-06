namespace ESFA.DC.Web.Operations.Models.Enums
{
    public enum FileNameValidationResult
    {
        Valid = 1,
        EmptyFile = 10,
        InvalidFileNameFormat = 20,
        InvalidFileExtension = 30,
        FileAlreadyExists = 40,
        LaterFileAlreadySubmitted = 50
    }
}
