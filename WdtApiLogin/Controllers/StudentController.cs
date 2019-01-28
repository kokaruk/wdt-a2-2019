using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using WdtApiLogin.Repo;

using WdtModels.ApiModels;

using WdtUtils.Model;

namespace WdtApiLogin.Controllers
{
    [Authorize(Roles = UserConstants.Student)]
    public class StudentController : Controller
    {
        private readonly IApiService _apiService;

        public StudentController(IApiService apiService) => this._apiService = apiService;
        
        public async Task<IActionResult> Index()
        {
            var users = await this._apiService.Users.GetAllAsync();

            var user = await this._apiService.Users.GetAsync("e12345");
            ViewBag.User = user;

            return View(users);
        }

        public async Task<IActionResult> StaffAvailability()
        {
            var user = await this._apiService.Users.GetAsync("e12345");
            return this.View();
        }

    }
}