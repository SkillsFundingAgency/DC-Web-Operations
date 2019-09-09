using System;
using System.Collections.Generic;
using Autofac;
using DC.Web.Authorization.Data.Repository;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Services;
using ESFA.DC.Web.Operations.Services.Hubs;
using ESFA.DC.Web.Operations.Services.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Topics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ESFA.DC.Web.Operations.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AzureStorageFileService>().As<IFileService>();

            builder.RegisterType<AuthorizeRepository>().As<IAuthorizeRepository>().InstancePerLifetimeScope();
            builder.RegisterType<PeriodEndService>().As<IPeriodEndService>().InstancePerLifetimeScope();
            builder.RegisterType<PeriodService>().As<IPeriodService>().InstancePerLifetimeScope();
            builder.RegisterType<StorageService>().As<IStorageService>().InstancePerLifetimeScope();

            builder.RegisterType<HubEventBase>().As<IHubEventBase>().SingleInstance();

            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().SingleInstance();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();

            builder.RegisterType<JobQueueDataContext>().InstancePerLifetimeScope();

            builder.Register(context =>
                {
                    var config = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<JobQueueDataContext>();
                    optionsBuilder.UseSqlServer(
                        config.JobManagement,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<JobQueueDataContext>>()
                .SingleInstance();
        }
    }
}