using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ESFA.DC.Web.Operations.Utils
{
    public static class RegexDefinitions
    {
        public static Regex ReportDate = new Regex(@"\d{8}-\d{6}", RegexOptions.Compiled);
    }
}
