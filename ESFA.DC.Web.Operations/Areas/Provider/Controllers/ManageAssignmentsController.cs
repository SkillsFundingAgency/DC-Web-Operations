using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Provider.Controllers
{
    public class ManageAssignmentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}