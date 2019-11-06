using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Ioc;
using ESFA.DC.Web.Operations.Services;
using ESFA.DC.Web.Operations.Services.Hubs;
using ESFA.DC.Web.Operations.Services.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.StartupConfiguration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace ESFA.DC.Web.Operations
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private IContainer _applicationContainer;
        private SeriLogger _logger;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(env.ContentRootPath);

            if (env.IsDevelopment())
            {
                builder.AddJsonFile($"appsettings.{Environment.UserName}.json");
            }
            else
            {
                builder.AddJsonFile("appsettings.json");
            }

            _config = builder.Build();

            var connectionStrings = _config.GetConfigSection<ConnectionStrings>();
            _logger = new SeriLogger(
                new ApplicationLoggerSettings
                {
                    ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>
                    {
                        new MsSqlServerApplicationLoggerOutputSettings
                        {
                            MinimumLogLevel = LogLevel.Verbose,
                            ConnectionString = connectionStrings.AppLogs,
                            LogsTableName = "Logs"
                        }
                    }
                },
                new ExecutionContext());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            _logger.LogDebug("Start of ConfigureServices");

            Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions aiOptions = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions();

            // Disables adaptive sampling.
            aiOptions.EnableAdaptiveSampling = false;

            // Disables QuickPulse (Live Metrics stream).
            aiOptions.EnableQuickPulseMetricStream = false;
            services.AddApplicationInsightsTelemetry(aiOptions);

            var authSettings = _config.GetConfigSection<AuthenticationSettings>();

            services.AddMvc()
                .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = true);

            services.AddAndConfigureAuthentication(authSettings);

            services.AddSignalR();

            services.AddHostedService<TimedHostedService>();

            services.AddHttpClient<IPeriodEndService, PeriodEndService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient<IPeriodService, PeriodService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            _logger.LogDebug("End of ConfigureServices");
            return ConfigureAutofac(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            _logger.LogDebug("Start of Configure");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<PeriodEndHub>("/periodEndHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });
                routes.MapHub<PeriodEndPrepHub>("/periodEndPrepHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });
                routes.MapHub<ProviderSearchHub>("/providerSearchHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "area",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            _logger.LogDebug("End of Configure");
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var jitter = new Random();

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    + TimeSpan.FromMilliseconds(jitter.Next(0, 100)));
        }

        private IServiceProvider ConfigureAutofac(IServiceCollection services)
        {
            _logger.LogDebug("Start of ConfigureAutofac");

            var containerBuilder = new ContainerBuilder();
            containerBuilder.SetupConfigurations(_config);

            containerBuilder.RegisterModule<ServiceRegistrations>();
            containerBuilder.RegisterModule<LoggerRegistrations>();

            containerBuilder.RegisterType<PeriodEndHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<PeriodEndPrepHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<ProviderSearchHub>().InstancePerLifetimeScope().ExternallyOwned();

            containerBuilder.Populate(services);
            _applicationContainer = containerBuilder.Build();

            _logger.LogDebug("End of ConfigureAutofac");
            return new AutofacServiceProvider(_applicationContainer);
        }
    }
}