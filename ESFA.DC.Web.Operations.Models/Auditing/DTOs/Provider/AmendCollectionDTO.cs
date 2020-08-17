using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Auditing.DTOs.Provider
{
   public class AmendCollectionDTO
    {
        public string CollectionType { get; set;  }

        public DateTime StartDateUTC { get; set;  }

        public DateTime EndDateUTC { get; set;  }
    }
}
