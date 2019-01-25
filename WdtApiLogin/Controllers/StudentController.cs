using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using WdtUtils.Model;

namespace WdtApiLogin.Controllers
{
    public class StudentController : Controller
    {
        private readonly IOptions<MyAppSettings> _appSettings;

        public StudentController(IOptions<MyAppSettings> appSettings)
        {
            this._appSettings = appSettings;
        }

        public IActionResult Index()
        {
            var list = new List<string> { "John", "Alex", this._appSettings.Value.WebApiUrl };

            return View(list);
        }
    }
}