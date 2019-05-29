using Newtonsoft.Json;

namespace ESFA.DC.Web.Operations.Settings.Models
{
    public class GoogleTrackingSettings
    {
        [JsonRequired]
        public string Key { get; set; }
    }
}
