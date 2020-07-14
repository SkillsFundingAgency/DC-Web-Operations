using System;
using System.Collections.Generic;
using ESFA.DC.Web.Operations.Interfaces;
using Flurl;

namespace ESFA.DC.Web.Operations.Services
{
    public class RouteFactory : IRouteFactory
    {
        public string BuildRoute(string baseUrl, IEnumerable<string> segments)
        {
            var clientUrl = baseUrl;

            if (segments != null)
            {
                foreach (var segment in segments)
                {
                    if (string.IsNullOrWhiteSpace(segment))
                    {
                        throw new ArgumentNullException("Path segments cannot be empty or null");
                    }

                    clientUrl = clientUrl.AppendPathSegment(segment);
                }
            }

            return clientUrl;
        }
    }
}