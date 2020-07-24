using System.Globalization;

namespace ESFA.DC.Web.Operations.Models.ServiceMessage
{
    public class ServiceMessageDto : Jobs.Model.ServiceMessageDto
    {
        public bool IsActive { get; set; }

        public string StartDateDisplayValue => StartDateTimeUtc.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture);

        public string StartTimeDisplayValue => StartDateTimeUtc.ToString("HH:mm", CultureInfo.CurrentCulture);

        public string EndDateDisplayValue => EndDateTimeUtc.HasValue ? EndDateTimeUtc.Value.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture) : string.Empty;

        public string EndTimeDisplayValue => EndDateTimeUtc.HasValue ? EndDateTimeUtc.Value.ToString("HH:mm", CultureInfo.CurrentCulture) : string.Empty;
    }
}
