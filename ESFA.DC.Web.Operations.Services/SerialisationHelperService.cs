using ESFA.DC.Web.Operations.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESFA.DC.Web.Operations.Services
{
    public class SerialisationHelperService : ISerialisationHelperService
    {
        public string SerialiseToCamelCase<T>(T model)
        {
            return JsonConvert.SerializeObject(
                model,
                Formatting.None,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }
    }
}