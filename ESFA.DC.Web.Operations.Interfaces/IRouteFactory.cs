using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IRouteFactory
    {
        string BuildRoute(string baseUrl, IEnumerable<string> segments);
    }
}