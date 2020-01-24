﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Constants.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    [Authorize(Policy = Constants.Authorization.AuthorisationPolicy.OpsPolicy)]
    public class ProviderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}