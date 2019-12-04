﻿using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.Dashboard;

namespace ESFA.DC.Web.Operations.Interfaces.Dashboard
{
    public interface IDashBoardService
    {
        Task<DashBoardModel> ProvideAsync(CancellationToken cancellationToken);
    }
}