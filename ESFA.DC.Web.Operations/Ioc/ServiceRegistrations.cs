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
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Auditing;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.Interfaces.Notifications;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Interfaces.Publication;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Interfaces.ValidationRules;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Services;
using ESFA.DC.Web.Operations.Services.Auditing;
using ESFA.DC.Web.Operations.Services.Builders;
using ESFA.DC.Web.Operations.Services.Collections;
using ESFA.DC.Web.Operations.Services.DashBoard;
using ESFA.DC.Web.Operations.Services.FileValidation;
using ESFA.DC.Web.Operations.Services.FileValidation.Providers;
using ESFA.DC.Web.Operations.Services.FileValidation.StandardValidator;
using ESFA.DC.Web.Operations.Services.Frm;
using ESFA.DC.Web.Operations.Services.Hubs;
using ESFA.DC.Web.Operations.Services.Hubs.ReferenceData;
using ESFA.DC.Web.Operations.Services.Notifications;
using ESFA.DC.Web.Operations.Services.PeriodEnd;
using ESFA.DC.Web.Operations.Services.PeriodEnd.ALLF;
using ESFA.DC.Web.Operations.Services.PeriodEnd.ILR;
using ESFA.DC.Web.Operations.Services.PeriodEnd.NCS;
using ESFA.DC.Web.Operations.Services.Processing;
using ESFA.DC.Web.Operations.Services.Provider;
using ESFA.DC.Web.Operations.Services.ReferenceData;
using ESFA.DC.Web.Operations.Services.Reports;
using ESFA.DC.Web.Operations.Services.Storage;
using ESFA.DC.Web.Operations.Services.ValidationRules;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.TagHelpers;
using ESFA.DC.Web.Operations.Topics.Data.Auditing;
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
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().As<ISerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<SerialisationHelperService>().As<ISerialisationHelperService>().InstancePerLifetimeScope();
            builder.RegisterType<RouteFactory>().As<IRouteFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CloudStorageService>().As<ICloudStorageService>().SingleInstance();

            builder.RegisterType<HubEventBase>().As<Interfaces.IHubEventBase>().SingleInstance();
            builder.RegisterType<PeriodEndPrepHubEventBase>().As<IPeriodEndPrepHubEventBase>().SingleInstance();
            builder.RegisterType<DashBoardHubEventBase>().As<IDashBoardHubEventBase>().SingleInstance();
            builder.RegisterType<JobQueuedHubEventBase>().As<IJobQueuedHubEventBase>().SingleInstance();
            builder.RegisterType<JobProcessingHubEventBase>().As<IJobProcessingHubEventBase>().SingleInstance();
            builder.RegisterType<JobSubmittedHubEventBase>().As<IJobSubmittedHubEventBase>().SingleInstance();
            builder.RegisterType<JobFailedTodayHubEventBase>().As<IJobFailedTodayHubEventBase>().SingleInstance();
            builder.RegisterType<JobSlowFileHubEventBase>().As<IJobSlowFileHubEventBase>().SingleInstance();
            builder.RegisterType<JobConcernHubEventBase>().As<IJobConcernHubEventBase>().SingleInstance();
            builder.RegisterType<JobDasMismatchHubEventBase>().As<IJobDasMismatchHubEventBase>().SingleInstance();
            builder.RegisterType<JobFailedCurrentPeriodHubEventBase>().As<IJobFailedCurrentPeriodHubEventBase>().SingleInstance();
            builder.RegisterType<ProvidersReturnedCurrentPeriodHubEventBase>().As<IJobProvidersReturnedCurrentPeriodHubEventBase>().SingleInstance();
            builder.RegisterType<ValidityPeriodHubEventBase>().As<IValidityPeriodHubEventBase>().SingleInstance();

            builder.RegisterType<ConditionOfFundingRemovalHub>().As<IReferenceDataHub>().SingleInstance();
            builder.RegisterType<FundingClaimsProviderDataHub>().As<IReferenceDataHub>().SingleInstance();
            builder.RegisterType<ProviderPostcodeSpecialistResourcesHub>().As<IReferenceDataHub>().SingleInstance();
            builder.RegisterType<CampusIdentifiersHub>().As<IReferenceDataHub>().SingleInstance();
            builder.RegisterType<ValidationErrorMessages2021Hub>().As<IReferenceDataHub>().SingleInstance();
            builder.RegisterType<DevolvedPostcodesHub>().As<IReferenceDataHub>().SingleInstance();
            builder.RegisterType<OnsPostcodesHub>().As<IReferenceDataHub>().SingleInstance();

            builder.RegisterType<DashBoardService>().As<IDashBoardService>().InstancePerLifetimeScope();

            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            builder.RegisterType<HttpClient>().SingleInstance();
            builder.RegisterType<BaseHttpClientService>().As<IHttpClientService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailService>().As<IEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<ILRHistoryService>().As<IILRHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<NCSHistoryService>().As<INCSHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<ALLFHistoryService>().As<IALLFHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<StateService>().As<IStateService>().InstancePerLifetimeScope();
            builder.RegisterType<JobStatusService>().As<IJobStatusService>().InstancePerLifetimeScope();

            builder.RegisterType<AddNewProviderService>().As<IAddNewProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<SearchProviderService>().As<ISearchProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<ManageProvidersService>().As<IManageProvidersService>().InstancePerLifetimeScope();
            builder.RegisterType<ManageAssignmentsService>().As<IManageAssignmentsService>().InstancePerLifetimeScope();
            builder.RegisterType<CollectionsService>().As<ICollectionsService>().InstancePerLifetimeScope();
            builder.RegisterType<JobService>().As<IJobService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationRulesService>().As<IValidationRulesService>().InstancePerLifetimeScope();
            builder.RegisterType<FundingClaimsDatesService>().As<IFundingClaimsDatesService>().InstancePerLifetimeScope();
            builder.RegisterType<ReferenceDataService>().As<IReferenceDataService>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationsService>().As<INotificationsService>().InstancePerLifetimeScope();

            builder.RegisterType<StandardFileNameValidationService>().As<IFileNameValidationService>().InstancePerLifetimeScope();
            builder.RegisterType<BulkProviderUploadFileNameValidationService>().As<IFileNameValidationService>().InstancePerLifetimeScope();
            builder.RegisterType<FileNameValidationServiceProvider>().As<IFileNameValidationServiceProvider>().InstancePerLifetimeScope();

            builder.RegisterType<FileUploadJobMetaDataModelBuilderService>().As<IFileUploadJobMetaDataModelBuilderService>().InstancePerLifetimeScope();

            builder.RegisterType<ReportsPublicationService>().As<IReportsPublicationService>().WithAttributeFiltering().InstancePerLifetimeScope();

            builder.RegisterType<AuditService>().As<IAuditService>().InstancePerLifetimeScope();
            builder.RegisterType<AuditRepository>().As<IAuditRepository>().InstancePerLifetimeScope();
            builder.RegisterType<DifferentiatorLookupService>().As<IDifferentiatorLookupService>().InstancePerLifetimeScope();

            builder.RegisterType<SeasonIconTagHelper>().As<SeasonIconTagHelper>().InstancePerLifetimeScope();

            builder.RegisterType<JobProcessingService>().As<IJobProcessingService>().InstancePerLifetimeScope();

            builder.RegisterType<JobProcessingDetailService>().As<IJobProcessingDetailService>().InstancePerLifetimeScope();

            builder.RegisterType<JobQueuedService>().As<IJobQueuedService>().InstancePerLifetimeScope();
            builder.RegisterType<JobSubmittedService>().As<IJobSubmittedService>().InstancePerLifetimeScope();
            builder.RegisterType<JobFailedTodayService>().As<IJobFailedTodayService>().InstancePerLifetimeScope();
            builder.RegisterType<JobSlowFileService>().As<IJobSlowFileService>().InstancePerLifetimeScope();
            builder.RegisterType<JobConcernService>().As<IJobConcernService>().InstancePerLifetimeScope();
            builder.RegisterType<JobDasMismatchService>().As<IJobDasMismatchService>().InstancePerLifetimeScope();
            builder.RegisterType<JobFailedCurrentPeriodService>().As<IJobFailedCurrentPeriodService>().InstancePerLifetimeScope();
            builder.RegisterType<ProvidersReturnedCurrentPeriodService>().As<IJobProvidersReturnedCurrentPeriodService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidityPeriodService>().As<IValidityPeriodService>().InstancePerLifetimeScope();

            // Reports
            builder.RegisterType<ACTCountReport>().As<IReport>().InstancePerLifetimeScope();
            builder.RegisterType<InternalDataMatchReport>().As<IReport>().InstancePerLifetimeScope();
            builder.RegisterType<ProviderSubmissionsReport>().As<IReport>().InstancePerLifetimeScope();
            builder.RegisterType<RuleValidationDetailReport>().As<IReport>().InstancePerLifetimeScope();
            builder.RegisterType<PeriodEndDataQualityReport>().As<IReport>().InstancePerLifetimeScope();
            builder.RegisterType<ILRProvidersReturningFirstTimePerDayReport>().As<IReport>().InstancePerLifetimeScope();

            // Collections
            builder.RegisterType<CampusIdentifiers>().As<ICollection>().InstancePerLifetimeScope();
            builder.RegisterType<CoFRemoval>().As<ICollection>().InstancePerLifetimeScope();
            builder.RegisterType<FundingClaimsProviderData>().As<ICollection>().InstancePerLifetimeScope();
            builder.RegisterType<OnsPostcodes>().As<ICollection>().InstancePerLifetimeScope();
            builder.RegisterType<ProviderPostcodeSpecialistResources>().As<ICollection>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationMessages2021>().As<ICollection>().InstancePerLifetimeScope();
            builder.RegisterType<DevolvedContracts>().As<ICollection>().InstancePerLifetimeScope();
            builder.RegisterType<DevolvedPostcodesFullName>().As<ICollection>().InstancePerLifetimeScope();
            builder.RegisterType<DevolvedPostcodesLocalAuthority>().As<ICollection>().InstancePerLifetimeScope();
            builder.RegisterType<DevolvedPostcodesOnsOverride>().As<ICollection>().InstancePerLifetimeScope();
            builder.RegisterType<DevolvedPostcodesSof>().As<ICollection>().InstancePerLifetimeScope();
            builder.RegisterType<RefOps>().As<ICollection>().InstancePerLifetimeScope();
            builder.RegisterType<ShortTermFundingInitiatives>().As<ICollection>().InstancePerLifetimeScope();

            // DB Contexts
            builder.RegisterType<JobQueueDataContext>().SingleInstance();
            builder.RegisterType<JobQueueManager.Data.JobQueueDataContext>().As<IJobQueueDataContext>().ExternallyOwned();
            builder.RegisterType<OrganisationsContext>().As<IOrganisationsContext>().ExternallyOwned();
            builder.RegisterType<AuditDataContext>().As<IAuditDataContext>().ExternallyOwned();

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

            builder.Register(context =>
            {
                var config = context.Resolve<ConnectionStrings>();
                var optionsBuilder = new DbContextOptionsBuilder<AuditDataContext>();
                optionsBuilder.UseSqlServer(
                    config.Audit,
                    options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                return optionsBuilder.Options;
            })
             .As<DbContextOptions<AuditDataContext>>()
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