﻿using System;
using System.IO;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
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

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Directory.GetCurrentDirectory());

            if (env.IsDevelopment())
            {
                builder.AddJsonFile($"appsettings.{Environment.UserName}.json");
            }
            else
            {
                builder.AddJsonFile("appsettings.json");
            }

            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var authSettings = _config.GetConfigSection<AuthenticationSettings>();

            services.AddMvc()
                .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = false);

            services.AddAndConfigureAuthentication(authSettings);

            services.AddSignalR();

            services.AddHostedService<TimedHostedService>();

            services.AddHttpClient<IPeriodEndService, PeriodEndService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            return ConfigureAutofac(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
                routes.MapHub<OperationsHub>("/operationsHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });
                routes.MapHub<PeriodEndHub>("/periodEndHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
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
            var containerBuilder = new ContainerBuilder();
            containerBuilder.SetupConfigurations(_config);

            containerBuilder.RegisterModule<ServiceRegistrations>();
            containerBuilder.RegisterModule<LoggerRegistrations>();

            containerBuilder.RegisterType<PeriodEndHub>().ExternallyOwned();

            containerBuilder.Populate(services);
            _applicationContainer = containerBuilder.Build();

            return new AutofacServiceProvider(_applicationContainer);
        }
    }
}