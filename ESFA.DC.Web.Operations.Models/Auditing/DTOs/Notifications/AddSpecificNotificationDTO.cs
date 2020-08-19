using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Auditing.DTOs.Notifications
{
   public class AddSpecificNotificationDTO
    {
        public string Details { get; set;  }

        public DateTime StartDateUTC { get; set;  }

        public DateTime EndDateUTC { get; set;  }
    }
}
