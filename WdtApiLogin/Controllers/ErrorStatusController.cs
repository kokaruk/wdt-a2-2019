using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WdtApiLogin.Controllers
{
    public class ErrorStatusController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index(int statusCode)
        {
            return View(statusCode);
        }
    }
}