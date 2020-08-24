using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Interfaces.Collections
{
    public interface ICollection
    {
        string CollectionName { get; }

        string ReportName { get; }

        string DisplayName { get; }

        string HubName { get; }

        string FileFormat { get; }
    }
}
