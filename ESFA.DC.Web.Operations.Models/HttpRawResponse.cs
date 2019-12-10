namespace ESFA.DC.Web.Operations.Models
{
    public class HttpRawResponse
    {
        public int StatusCode { get; set; }

        public bool IsSuccess { get; set; }

        public string Content { get; set; }
    }
}
