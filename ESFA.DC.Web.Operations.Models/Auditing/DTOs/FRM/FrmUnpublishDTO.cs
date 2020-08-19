using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Auditing.DTOs.FRM
{
    public class FrmUnpublishDTO
    {
        public int Period { get; set;  }

        public int AcademicYear { get; set;  }

        public int CollectionYear { get; set;  }

        public string Folder { get; set;  }
    }
}
