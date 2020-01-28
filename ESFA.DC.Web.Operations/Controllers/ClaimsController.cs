using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    [Route("claims")]
    public class ClaimsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}