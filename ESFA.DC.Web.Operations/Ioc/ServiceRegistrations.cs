using System;
using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Config;
using ESFA.DC.FileService.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.ReferenceData.Organisations.Model;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Web.Operations.Areas.Frm.Controllers;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.Interfaces.Frm;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Interfaces.Reports;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Interfaces.ValidationRules;
using ESFA.DC.Web.Operations.Services;
using ESFA.DC.Web.Operations.Services.Collections;
using ESFA.DC.Web.Operations.Services.DashBoard;
using ESFA.DC.Web.Operations.Services.Frm;
using ESFA.DC.Web.Operations.Services.Hubs;
using ESFA.DC.Web.Operations.Services.PeriodEnd;
using ESFA.DC.Web.Operations.Services.PeriodEnd.ALLF;
using ESFA.DC.Web.Operations.Services.PeriodEnd.ILR;
using ESFA.DC.Web.Operations.Services.PeriodEnd.NCS;
using ESFA.DC.Web.Operations.Services.Processing;
using ESFA.DC.Web.Operations.Services.Provider;
using ESFA.DC.Web.Operations.Services.Reports;
using ESFA.DC.Web.Operations.Services.Storage;
using ESFA.DC.Web.Operations.Services.ValidationRules;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.TagHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using JobQueueDataContext = ESFA.DC.Web.Operations.Topics.Data.JobQueueDataContext;

namespace ESFA.DC.Web.Operations.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PeriodEndService>().As<IPeriodEndService>().InstancePerLifetimeScope();
            builder.RegisterType<ALLFPeriodEndService>().As<IALLFPeriodEndService>().InstancePerLifetimeScope();
            builder.RegisterType<NCSPeriodEndService>().As<INCSPeriodEndService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailDistributionService>().As<IEmailDistributionService>().InstancePerLifetimeScope();
            builder.RegisterType<PeriodService>().As<IPeriodService>().InstancePerLifetimeScope();
            builder.RegisterType<StorageService>().As<IStorageService>().WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();

            builder.RegisterType<PeriodEndHubEventBase>().As<IPeriodEndHubEventBase>().SingleInstance();
            builder.RegisterType<PeriodEndPrepHubEventBase>().As<IPeriodEndPrepHubEventBase>().SingleInstance();
            builder.RegisterType<DashBoardHubEventBase>().As<IDashBoardHubEventBase>().SingleInstance();
            builder.RegisterType<JobQueuedHubEventBase>().As<IJobQueuedHubEventBase>().SingleInstance();
            builder.RegisterType<JobProcessingHubEventBase>().As<IJobProcessingHubEventBase>().SingleInstance();
            builder.RegisterType<JobSubmittedHubEventBase>().As<IJobSubmittedHubEventBase>().SingleInstance();
            builder.RegisterType<JobFailedTodayHubEventBase>().As<IJobFailedTodayHubEventBase>().SingleInstance();
            builder.RegisterType<JobSlowFileHubEventBase>().As<IJobSlowFileHubEventBase>().SingleInstance();
            builder.RegisterType<JobConcernHubEventBase>().As<IJobConcernHubEventBase>().SingleInstance();
            builder.RegisterType<JobDasMismatchHubEventBase>().As<IJobDasMismatchHubEventBase>().SingleInstance();
            builder.RegisterType<ValidityPeriodHubEventBase>().As<IValidityPeriodHubEventBase>().SingleInstance();

            builder.RegisterType<DashBoardService>().As<IDashBoardService>().InstancePerLifetimeScope();

            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            builder.RegisterType<HttpClient>().SingleInstance();
            builder.RegisterType<EmailService>().As<IEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<ILRHistoryService>().As<IILRHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<NCSHistoryService>().As<INCSHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<StateService>().As<IStateService>().InstancePerLifetimeScope();

            builder.RegisterType<AddNewProviderService>().As<IAddNewProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<SearchProviderService>().As<ISearchProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<ManageProvidersService>().As<IManageProvidersService>().InstancePerLifetimeScope();
            builder.RegisterType<ManageAssignmentsService>().As<IManageAssignmentsService>().InstancePerLifetimeScope();
            builder.RegisterType<CollectionsService>().As<ICollectionsService>().InstancePerLifetimeScope();
            builder.RegisterType<JobService>().As<IJobService>().InstancePerLifetimeScope();
            builder.RegisterType<FileNameValidationService>().As<IFileNameValidationService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationRulesService>().As<IValidationRulesService>().InstancePerLifetimeScope();

            builder.RegisterType<FrmService>().As<IFrmService>().WithAttributeFiltering().InstancePerLifetimeScope();

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().InstancePerLifetimeScope();

            builder.RegisterType<SeasonIconTagHelper>().As<SeasonIconTagHelper>().InstancePerLifetimeScope();

            builder.RegisterType<JobProcessingService>().As<IJobProcessingService>().InstancePerLifetimeScope();

            builder.RegisterType<JobProcessingDetailService>().As<IJobProcessingDetailService>().InstancePerLifetimeScope();

            builder.RegisterType<JobQueuedService>().As<IJobQueuedService>().InstancePerLifetimeScope();
            builder.RegisterType<JobSubmittedService>().As<IJobSubmittedService>().InstancePerLifetimeScope();
            builder.RegisterType<JobFailedTodayService>().As<IJobFailedTodayService>().InstancePerLifetimeScope();
            builder.RegisterType<JobSlowFileService>().As<IJobSlowFileService>().InstancePerLifetimeScope();
            builder.RegisterType<JobConcernService>().As<IJobConcernService>().InstancePerLifetimeScope();
            builder.RegisterType<JobDasMismatchService>().As<IJobDasMismatchService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidityPeriodService>().As<IValidityPeriodService>().InstancePerLifetimeScope();

            // DB Contexts
            builder.RegisterType<JobQueueDataContext>().SingleInstance();
            builder.RegisterType<JobQueueManager.Data.JobQueueDataContext>().As<IJobQueueDataContext>().ExternallyOwned();
            builder.RegisterType<OrganisationsContext>().As<IOrganisationsContext>().ExternallyOwned();

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

            builder.Register(context =>
                {
                    var config = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<JobQueueManager.Data.JobQueueDataContext>();
                    optionsBuilder.UseSqlServer(
                        config.JobManagement,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<JobQueueManager.Data.JobQueueDataContext>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var config = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<OrganisationsContext>();
                    optionsBuilder.UseSqlServer(
                        config.Org,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<OrganisationsContext>>()
                .SingleInstance();

            RegisterAzureStorage(builder);
        }

        private static void RegisterAzureStorage(ContainerBuilder builder)
        {
            builder.Register(c =>
                    new AzureStorageFileService(
                        new AzureStorageFileServiceConfiguration
                        {
                            ConnectionString = c.Resolve<AzureStorageSection>().ConnectionString
                        }))
                .Keyed<IFileService>(PersistenceStorageKeys.DctAzureStorage)
                .SingleInstance();

            builder.Register(c =>
                    new AzureStorageFileService(
                        new AzureStorageFileServiceConfiguration
                        {
                            ConnectionString = c.Resolve<OpsDataLoadServiceConfigSettings>().ConnectionString
                        }))
                .Keyed<IFileService>(PersistenceStorageKeys.OperationsAzureStorage)
                .SingleInstance();
        }
    }
}