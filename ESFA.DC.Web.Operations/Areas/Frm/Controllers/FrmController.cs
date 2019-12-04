using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Frm.Controllers
{
    [Area(AreaNames.Frm)]
    public class FrmController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}