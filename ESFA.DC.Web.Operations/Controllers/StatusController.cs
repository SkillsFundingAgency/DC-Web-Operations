using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    [AllowAnonymous]
    public class StatusController : Controller
    {
        public StatusController()
        {
            Message = string.Empty;
        }

        [ViewData]
        public string Message { get; set; }

        public IActionResult Index()
        {
            Message = "OK";
            return View();
        }
    }
}