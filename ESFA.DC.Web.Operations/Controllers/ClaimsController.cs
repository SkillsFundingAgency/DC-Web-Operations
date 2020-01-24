using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Constants.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    public class ClaimsController : Controller
    {
        public ClaimsController()
        {
            Message = string.Empty;
        }

        [ViewData]
        public string Message { get; set; }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = AuthorisationPolicy.OpsPolicy)]
        public IActionResult Ops()
        {
            Message = "Ops";
            return View("Index");
        }

        [Authorize(Policy = AuthorisationPolicy.DevOpsPolicy)]
        public IActionResult DevOps()
        {
            Message = "Dev Ops";
            return View("Index");
        }
    }
}