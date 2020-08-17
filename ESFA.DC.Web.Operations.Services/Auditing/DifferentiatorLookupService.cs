using System;
using ESFA.DC.Web.Operations.Interfaces.Auditing;
using ESFA.DC.Web.Operations.Models.Auditing;

namespace ESFA.DC.Web.Operations.Services.Auditing
{
    public class DifferentiatorLookupService : IDifferentiatorLookupService
    {
        public int DifferentiatorLookup<T>() => (int)Enum.Parse(typeof(DifferentiatorPath), typeof(T).Name);
    }
}
