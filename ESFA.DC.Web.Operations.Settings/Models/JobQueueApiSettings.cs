using Newtonsoft.Json;

namespace ESFA.DC.Web.Operations.Settings.Models
{
    public class JobQueueApiSettings
    {
        [JsonRequired]
        public string BaseUrl { get; set; }

        public string ServiceFabricUrl { get; set; }

        public string Certificate { get; set; }
    }
}