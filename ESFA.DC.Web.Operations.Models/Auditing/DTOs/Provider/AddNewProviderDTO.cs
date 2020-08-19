using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Auditing.DTOs.Provider
{
    public class AddNewProviderDTO
    {
        public string Name { get; set; }

        public string UKPRN { get; set; }

        public int UPIN { get; set; }

        public bool IsMCA { get; set; }
    }
}