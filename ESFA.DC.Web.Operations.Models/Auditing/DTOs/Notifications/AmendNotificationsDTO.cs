using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Auditing.DTOs.Notifications
{
    public class AmendNotificationsDTO
    {
        public string MessageType { get; set;  }

        public string Details { get; set;  }

        public DateTime StartDateUTC { get; set;  }

        public DateTime EndDateUTC { get; set;  }
    }
}
