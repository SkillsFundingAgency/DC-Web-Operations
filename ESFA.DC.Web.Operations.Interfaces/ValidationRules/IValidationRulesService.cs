﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.ValidationRules
{
    public interface IValidationRulesService
    {
        Task<IEnumerable<string>> GetValidationRules(int year, CancellationToken cancellationToken = default(CancellationToken));

        Task<long> GenerateReport(string rule, int year, string createdBy, CancellationToken cancellationToken = default(CancellationToken));
    }
}
