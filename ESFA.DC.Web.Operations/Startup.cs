using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.Frm;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Reports;
using ESFA.DC.Web.Operations.Ioc;
using ESFA.DC.Web.Operations.Services;
using ESFA.DC.Web.Operations.Services.Frm;
using ESFA.DC.Web.Operations.Services.Hubs;
using ESFA.DC.Web.Operations.Services.Hubs.PeriodEnd.ALLF;
using ESFA.DC.Web.Operations.Services.Hubs.PeriodEnd.ILR;
using ESFA.DC.Web.Operations.Services.Hubs.PeriodEnd.NCS;
using ESFA.DC.Web.Operations.Services.Hubs.ReferenceData;
using ESFA.DC.Web.Operations.Services.PeriodEnd;
using ESFA.DC.Web.Operations.Services.PeriodEnd.ALLF;
using ESFA.DC.Web.Operations.Services.PeriodEnd.NCS;
using ESFA.DC.Web.Operations.Services.ReferenceData;
using ESFA.DC.Web.Operations.Services.Reports;
using ESFA.DC.Web.Operations.Services.TimedHostedService.ALLF;
using ESFA.DC.Web.Operations.Services.TimedHostedService.ILR;
using ESFA.DC.Web.Operations.Services.TimedHostedService.NCS;
using ESFA.DC.Web.Operations.Services.TimedHostedService.ReferenceData;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.StartupConfiguration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Features;
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

            builder.AddJsonFile("appsettings.json");
            builder.AddJsonFile($"appsettings.{Environment.UserName}.json", optional: true);

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

            CultureInfo.CurrentCulture = new CultureInfo("en-GB");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            _logger.LogDebug("Start of ConfigureServices");

            services.AddApplicationInsightsTelemetry();

            var authSettings = _config.GetConfigSection<AuthenticationSettings>();
            var authoriseSettings = _config.GetConfigSection<AuthorizationSettings>();

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = 524_288_000;
                x.MultipartBodyLengthLimit = 524_288_000;
                x.MultipartBoundaryLengthLimit = 524_288_000;
            });

            services.AddMvc(options =>
                            {
                                options.Filters.Add(typeof(CustomFilters.TelemetryActionFilter));
                            })
                    .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = true);

            services.AddAndConfigureAuthentication(authSettings);
            services.AddAndConfigureAuthorisation(authoriseSettings);

            services.AddSignalR();

            services.AddHostedService<PeriodEndPrepTimedHostedService>();
            services.AddHostedService<PeriodEndTimedHostedService>();
            services.AddHostedService<DashboardTimedHostedService>();
            services.AddHostedService<JobProcessingTimedHostedService>();
            services.AddHostedService<JobQueuedTimedHostedService>();
            services.AddHostedService<JobSubmittedTimedHostedService>();
            services.AddHostedService<JobFailedTodayTimedHostedService>();
            services.AddHostedService<JobSlowFileTimedHostedService>();
            services.AddHostedService<JobConcernTimedHostedService>();
            services.AddHostedService<JobDasMismatchTimedHostedService>();

            services.AddHostedService<NCSPeriodEndPrepTimedHostedService>();
            services.AddHostedService<NCSPeriodEndTimedHostedService>();

            services.AddHostedService<ALLFPeriodEndTimedHostedService>();

            services.AddHostedService<ReferenceDataTimedHostedService>();

            services.AddHttpClient<IPeriodEndService, PeriodEndService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient<IPeriodService, PeriodService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient<IReportsService, ReportsService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient<IFrmService, FrmService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient<INCSPeriodEndService, NCSPeriodEndService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient<IALLFPeriodEndService, ALLFPeriodEndService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient<IReferenceDataService, ReferenceDataService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpContextAccessor();

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
                routes.MapHub<DashBoardHub>("/dashBoardHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });
                routes.MapHub<JobProcessingHub>("/jobProcessingHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<JobProcessingDetailHub>("/jobProcessingDetailHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<JobQueuedHub>("/jobQueuedHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<ReportsHub>("/reportsHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<FundingClaimsDatesHub>("/fundingclaimsdateshub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<JobSubmittedHub>("/jobSubmittedHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<JobFailedTodayHub>("/jobFailedTodayHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<JobSlowFileHub>("/jobSlowFileHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<JobConcernHub>("/jobConcernHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<ValidityPeriodHub>("/validityPeriodHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<NCSPeriodEndPrepHub>("/ncsPeriodEndPrepHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });
                routes.MapHub<NCSPeriodEndHub>("/ncsPeriodEndHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<ALLFPeriodEndHub>("/allfPeriodEndHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<CampusIdentifiersHub>("/campusIdentifiersHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<JobDasMismatchHub>("/jobDasMismatchHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<JobFailedCurrentPeriodHub>("/jobFailedCurrentPeriodHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<ProvidersReturnedCurrentPeriodHub>("/jobProvidersReturnedCurrentPeriodHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<ConditionOfFundingRemovalHub>("/conditionOfFundingRemovalHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<FundingClaimsProviderDataHub>("/fundingClaimsProviderDataHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<ProviderPostcodeSpecialistResourcesHub>("/providerPostcodeSpecialistResourcesHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<DevolvedPostcodesHub>("/devolvedPostcodesHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<OnsPostcodesHub>("/onsPostcodesHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                routes.MapHub<ValidationErrorMessages2021Hub>("/validationMessages2021Hub", options =>
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

            containerBuilder.RegisterType<NCSPeriodEndPrepHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<NCSPeriodEndHub>().InstancePerLifetimeScope().ExternallyOwned();

            containerBuilder.RegisterType<ALLFPeriodEndHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<CampusIdentifiersHub>().InstancePerLifetimeScope().ExternallyOwned();

            containerBuilder.RegisterType<ProviderSearchHub>().InstancePerLifetimeScope().ExternallyOwned();

            containerBuilder.RegisterType<DashBoardHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<JobProcessingHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<JobQueuedHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<JobSubmittedHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<JobFailedTodayHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<JobSlowFileHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<JobConcernHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<JobDasMismatchHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<JobFailedCurrentPeriodHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<ProvidersReturnedCurrentPeriodHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<ValidityPeriodHub>().InstancePerLifetimeScope().ExternallyOwned();

            containerBuilder.RegisterType<ConditionOfFundingRemovalHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<FundingClaimsProviderDataHub>().InstancePerLifetimeScope().ExternallyOwned();
            containerBuilder.RegisterType<ProviderPostcodeSpecialistResourcesHub>().InstancePerLifetimeScope().ExternallyOwned();

            containerBuilder.Populate(services);
            _applicationContainer = containerBuilder.Build();

            var opsConnectionString = _applicationContainer.Resolve<OpsDataLoadServiceConfigSettings>().ConnectionString;
            var azureConnectionString = _applicationContainer.Resolve<AzureStorageSection>().ConnectionString;

            _logger.LogDebug("End of ConfigureAutofac");

            return new AutofacServiceProvider(_applicationContainer);
        }
    }
}