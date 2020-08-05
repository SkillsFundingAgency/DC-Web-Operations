using ESFA.DC.Web.Operations.Interfaces.Auditing;
using ESFA.DC.Web.Operations.Topics.Data.Auditing.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Topics.Data.Auditing
{
   public class AuditDataContext: DbContext, IDisposable, IAuditDataContext
    {
        public AuditDataContext(DbContextOptions<AuditDataContext> options)
           : base(options)
        {
        }

        public virtual DbSet<Audit> Audit { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(string.Empty);
            }
        }
    }
}
