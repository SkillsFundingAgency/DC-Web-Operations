using Newtonsoft.Json;

namespace ESFA.DC.Web.Operations.Settings.Models
{
    public class AuthenticationSettings : ISettings
    {
        [JsonRequired]
        public string WtRealm { get; set; }

        [JsonRequired]
        public string MetadataAddress { get; set; }
    }
}