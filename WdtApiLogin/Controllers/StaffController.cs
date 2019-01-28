using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WdtUtils.Model;

namespace WdtApiLogin.Controllers
{
    [Authorize(Roles = UserConstants.Staff)]
    public class StaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoomAvailability()
        {
            return View();
        }

    }
}