using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Web.Operations.Entities
{
    public partial class JobQueueDataContext : DbContext
    {
        public JobQueueDataContext()
        {
        }

        public JobQueueDataContext(DbContextOptions<JobQueueDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Collection> Collection { get; set; }

        public virtual DbSet<CollectionType> CollectionType { get; set; }

        public virtual DbSet<EasJobMetaData> EasJobMetaData { get; set; }

        public virtual DbSet<FileUploadJobMetaData> FileUploadJobMetaData { get; set; }

        public virtual DbSet<FundingClaimsReturnPeriodMetaData> FundingClaimsReturnPeriodMetaData { get; set; }

        public virtual DbSet<IlrJobMetaData> IlrJobMetaData { get; set; }

        public virtual DbSet<Job> Job { get; set; }

        public virtual DbSet<JobEmailTemplate> JobEmailTemplate { get; set; }

        public virtual DbSet<JobMessageKey> JobMessageKey { get; set; }

        public virtual DbSet<JobStatusType> JobStatusType { get; set; }

        public virtual DbSet<JobSubscriptionTask> JobSubscriptionTask { get; set; }

        public virtual DbSet<JobTopicSubscription> JobTopicSubscription { get; set; }

        public virtual DbSet<NcsJobMetaData> NcsJobMetaData { get; set; }

        public virtual DbSet<Organisation> Organisation { get; set; }

        public virtual DbSet<OrganisationCollection> OrganisationCollection { get; set; }

        public virtual DbSet<Path> Path { get; set; }

        public virtual DbSet<PathItem> PathItem { get; set; }

        public virtual DbSet<PathItemJob> PathItemJob { get; set; }

        public virtual DbSet<PeriodEnd> PeriodEnd { get; set; }

        public virtual DbSet<ReturnPeriod> ReturnPeriod { get; set; }

        public virtual DbSet<ReturnPeriodDisplayOverride> ReturnPeriodDisplayOverride { get; set; }

        public virtual DbSet<ReturnPeriodOrganisationOverride> ReturnPeriodOrganisationOverride { get; set; }

        public virtual DbSet<Schedule> Schedule { get; set; }

        public virtual DbSet<ServiceMessage> ServiceMessage { get; set; }

        // Unable to generate entity type for table 'DataLoad.ILR1920_Standard'. Please see the warning messages.
        // Unable to generate entity type for table 'DataLoad.ESF_R1'. Please see the warning messages.
        // Unable to generate entity type for table 'DataLoad.EAS1819'. Please see the warning messages.
        // Unable to generate entity type for table 'DataLoad.ILR1819_Standard'. Please see the warning messages.
        // Unable to generate entity type for table 'DataLoad.FundingClaim'. Please see the warning messages.
        // Unable to generate entity type for table 'DataLoad.ILR1920_Periodic'. Please see the warning messages.
        // Unable to generate entity type for table 'DataLoad.ILR1819_Periodic'. Please see the warning messages.
        // Unable to generate entity type for table 'DataLoad.ESF-R2'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=(local);Database=JobManagement;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Collection>(entity =>
            {
                entity.Property(e => e.CollectionId).ValueGeneratedNever();

                entity.Property(e => e.CollectionYear).HasDefaultValueSql("((1819))");

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StorageReference).HasMaxLength(100);

                entity.Property(e => e.SubText)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.CollectionType)
                    .WithMany(p => p.Collection)
                    .HasForeignKey(d => d.CollectionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Collection_CollectionType");
            });

            modelBuilder.Entity<CollectionType>(entity =>
            {
                entity.Property(e => e.CollectionTypeId).ValueGeneratedNever();

                entity.Property(e => e.ConcurrentExecutionCount).HasDefaultValueSql("((25))");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EasJobMetaData>(entity =>
            {
                entity.HasOne(d => d.Job)
                    .WithMany(p => p.EasJobMetaData)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EasJobMetaData_ToJob");
            });

            modelBuilder.Entity<FileUploadJobMetaData>(entity =>
            {
                entity.HasIndex(e => e.JobId)
                    .HasName("IX_FileUploadJobMetaData_Column");

                entity.Property(e => e.FileName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.FileSize).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PeriodNumber).HasDefaultValueSql("((1))");

                entity.Property(e => e.StorageReference)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.FileUploadJobMetaData)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FileUploadJobMetaData_ToJob");
            });

            modelBuilder.Entity<FundingClaimsReturnPeriodMetaData>(entity =>
            {
                entity.Property(e => e.SignatureCloseDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.ReturnPeriod)
                    .WithMany(p => p.FundingClaimsReturnPeriodMetaData)
                    .HasForeignKey(d => d.ReturnPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FundingClaimsReturnPeriodMetaData_ReturnPeriod");
            });

            modelBuilder.Entity<IlrJobMetaData>(entity =>
            {
                entity.HasIndex(e => new { e.DateTimeSubmittedUtc, e.JobId })
                    .HasName("IX_IlrMetaData_JobId");

                entity.Property(e => e.DateTimeSubmittedUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.IlrJobMetaData)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IlrJobMetaData_IlrJobMetaData");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateTimeCreatedUtc)
                    .HasColumnName("DateTimeCreatedUTC")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateTimeUpdatedUtc)
                    .HasColumnName("DateTimeUpdatedUTC")
                    .HasColumnType("datetime");

                entity.Property(e => e.NotifyEmail).HasMaxLength(500);

                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.Job)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Job_Collection");
            });

            modelBuilder.Entity<JobEmailTemplate>(entity =>
            {
                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CollectionId).HasDefaultValueSql("((1))");

                entity.Property(e => e.TemplateClosePeriod)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TemplateOpenPeriod)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.JobEmailTemplate)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobEmailTemplate_ToCollection");
            });

            modelBuilder.Entity<JobMessageKey>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.MessageKey)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<JobStatusType>(entity =>
            {
                entity.HasKey(e => e.StatusId);

                entity.Property(e => e.StatusId).ValueGeneratedNever();

                entity.Property(e => e.StatusDescription)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.StatusTitle)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<JobSubscriptionTask>(entity =>
            {
                entity.HasKey(e => e.JobTopicTaskId);

                entity.HasIndex(e => e.JobTopicTaskId)
                    .HasName("IX_JobSubscriptionTask")
                    .IsUnique();

                entity.Property(e => e.JobTopicTaskId).ValueGeneratedNever();

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.TaskName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TaskOrder).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.JobTopic)
                    .WithMany(p => p.JobSubscriptionTask)
                    .HasForeignKey(d => d.JobTopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobSubscriptionTask_JobTopic");
            });

            modelBuilder.Entity<JobTopicSubscription>(entity =>
            {
                entity.HasKey(e => e.JobTopicId);

                entity.HasIndex(e => e.JobTopicId)
                    .HasName("IX_JobTopicSubscription")
                    .IsUnique();

                entity.Property(e => e.JobTopicId).ValueGeneratedNever();

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.SubscriptionName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TopicName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TopicOrder).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.JobTopicSubscription)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobTopicSubscription_ToCollection");
            });

            modelBuilder.Entity<NcsJobMetaData>(entity =>
            {
                entity.Property(e => e.ExternalJobId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ExternalTimestamp).HasColumnType("datetime");

                entity.Property(e => e.ReportFileName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TouchpointId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.NcsJobMetaData)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NcsJobMetaData_ToJob");
            });

            modelBuilder.Entity<Organisation>(entity =>
            {
                entity.Property(e => e.OrganisationId).ValueGeneratedNever();

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsMca).HasColumnName("IsMCA");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OrganisationCollection>(entity =>
            {
                entity.HasKey(e => new { e.OrganisationId, e.CollectionId });

                entity.Property(e => e.EndDateTimeUtc)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('2600-07-31')");

                entity.Property(e => e.StartDateTimeUtc)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('2018-08-01')");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.OrganisationCollection)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganisationCollection_Collection");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.OrganisationCollection)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganisationCollection_Organisation");
            });

            modelBuilder.Entity<Path>(entity =>
            {
                entity.ToTable("Path", "PeriodEnd");

                entity.HasOne(d => d.PeriodEnd)
                    .WithMany(p => p.Path)
                    .HasForeignKey(d => d.PeriodEndId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Path_PeriodEnd");
            });

            modelBuilder.Entity<PathItem>(entity =>
            {
                entity.ToTable("PathItem", "PeriodEnd");

                entity.HasOne(d => d.Path)
                    .WithMany(p => p.PathItem)
                    .HasForeignKey(d => d.PathId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PathItem_Path");
            });

            modelBuilder.Entity<PathItemJob>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.PathItemId });

                entity.ToTable("PathItemJob", "PeriodEnd");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.PathItemJob)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PathItemJob_Job");

                entity.HasOne(d => d.PathItem)
                    .WithMany(p => p.PathItemJob)
                    .HasForeignKey(d => d.PathItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PathItemJob_PathItem");
            });

            modelBuilder.Entity<PeriodEnd>(entity =>
            {
                entity.ToTable("PeriodEnd", "PeriodEnd");

                entity.HasOne(d => d.Period)
                    .WithMany(p => p.PeriodEnd)
                    .HasForeignKey(d => d.PeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PeriodEnd_Period");
            });

            modelBuilder.Entity<ReturnPeriod>(entity =>
            {
                entity.HasIndex(e => new { e.CollectionId, e.ReturnPeriodId })
                    .HasName("UC_ReturnPeriod_Key")
                    .IsUnique();

                entity.Property(e => e.EndDateTimeUtc)
                    .HasColumnName("EndDateTimeUTC")
                    .HasColumnType("datetime");

                entity.Property(e => e.StartDateTimeUtc)
                    .HasColumnName("StartDateTimeUTC")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.ReturnPeriod)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReturnPeriod_Collection");
            });

            modelBuilder.Entity<ReturnPeriodDisplayOverride>(entity =>
            {
                entity.Property(e => e.EndDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.StartDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.ReturnPeriod)
                    .WithMany(p => p.ReturnPeriodDisplayOverride)
                    .HasForeignKey(d => d.ReturnPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReturnPeriodOverride_ReturnPeriod");
            });

            modelBuilder.Entity<ReturnPeriodOrganisationOverride>(entity =>
            {
                entity.HasOne(d => d.Orgaisation)
                    .WithMany(p => p.ReturnPeriodOrganisationOverride)
                    .HasForeignKey(d => d.OrgaisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReturnPeriodOrganisationOverride_Organisation");

                entity.HasOne(d => d.ReturnPeriod)
                    .WithMany(p => p.ReturnPeriodOrganisationOverride)
                    .HasForeignKey(d => d.ReturnPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReturnPeriodOrganisationOverride_ReturnPeriod");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CollectionId).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastExecuteDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.Schedule)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Schedule_Collection");
            });

            modelBuilder.Entity<ServiceMessage>(entity =>
            {
                entity.Property(e => e.EndDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.StartDateTimeUtc).HasColumnType("datetime");
            });
        }
    }
}
