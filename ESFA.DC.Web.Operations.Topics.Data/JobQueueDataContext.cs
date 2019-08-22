using ESFA.DC.Web.Operations.Topics.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Web.Operations.Topics.Data
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

        public virtual DbSet<JobSubscriptionTask> JobSubscriptionTask { get; set; }

        public virtual DbSet<JobTopicSubscription> JobTopicSubscription { get; set; }

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
        }
    }
}
