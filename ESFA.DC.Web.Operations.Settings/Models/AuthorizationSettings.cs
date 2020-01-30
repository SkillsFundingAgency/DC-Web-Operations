using ESFA.DC.Web.Operations.Interfaces.Settings;
using Newtonsoft.Json;

namespace ESFA.DC.Web.Operations.Settings.Models
{
    public class AuthorizationSettings : ISettings
    {
        [JsonRequired]
        public string OpsClaim { get; set; }

        [JsonRequired]
        public string DevOpsClaim { get; set; }
    }
}