using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Auditing.DTOs.Collections
{
    public class ManagingPeriodCollectionDTO
    {
        public DateTime OpeningDateUTC { get; set;  }

        public DateTime ClosingDateUTC { get; set;  }
    }
}