using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Auditing.DTOs.PeriodEnd
{
    public class AmendPeriodEndDTO
    {
        public int CollectionYear { get; set;  }

        public int Period { get; set;  }

        public string CollectionType { get; set;  }
    }
}
