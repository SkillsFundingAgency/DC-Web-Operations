using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Auditing.DTOs.FRM
{
   public class FrmValidateDTO
    {
        public string FrmContainerName { get; set;  }

        public string FrmFolderKey { get; set;  }

        public int FrmPeriodNumber { get; set;  }

        public string CurrentContainerName { get; set;  }
    }
}
