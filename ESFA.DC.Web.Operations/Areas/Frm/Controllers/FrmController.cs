using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Frm.Controllers
{
    public class FrmController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}