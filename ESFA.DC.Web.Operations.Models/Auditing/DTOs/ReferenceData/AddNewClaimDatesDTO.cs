﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Auditing.DTOs.ReferenceData
{
    public class AddNewClaimDatesDTO
    {
        public string CollectionName { get; set;  }

        public DateTime StartDateUTC { get; set;  }

        public DateTime EndDateUTC { get; set;  }
    }
}
