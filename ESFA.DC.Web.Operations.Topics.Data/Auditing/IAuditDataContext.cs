using Microsoft.EntityFrameworkCore;
using ESFA.DC.Web.Operations.Topics.Data.Auditing.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Topics.Data.Auditing
{
    public interface IAuditDataContext : IDisposable
    {
        DbSet<Audit> Audit { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
