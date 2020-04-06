using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ESFA.DC.Web.Operations.CustomFilters
{
    public class TelemetryActionFilter : IAsyncActionFilter
    {
        private readonly TelemetryClient _telemetryClient;

        public TelemetryActionFilter(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            SendTelemetry(context);
            var resultContext = await next();
        }

        private void SendTelemetry(ActionExecutingContext context)
        {
            try
            {
                var telemetryArgs = new Dictionary<string, string>();
                foreach (var arg in context.ActionArguments)
                {
                    telemetryArgs.Add(arg.Key, arg.Value.ToString());
                }

                _telemetryClient.TrackTrace(context.ActionDescriptor.DisplayName, telemetryArgs);
            }
            catch
            { }
        }
    }
}