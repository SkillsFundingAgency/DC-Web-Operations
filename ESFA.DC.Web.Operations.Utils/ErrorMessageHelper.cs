namespace ESFA.DC.Web.Operations.Utils
{
    public static class ErrorMessageHelper
    {
        public static string CreateErrorMessage(
            string message,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            return $"{sourceFilePath}.{memberName} line {sourceLineNumber}: {message}.";
        }
    }
}
