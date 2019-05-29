using Newtonsoft.Json;

namespace ESFA.DC.Web.Operations.Settings.Models
{
    public class FeatureFlags
    {
        [JsonRequired]
        public bool DuplicateFileCheckEnabled { get; set; }
    }
}
