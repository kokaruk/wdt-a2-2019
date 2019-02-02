using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WdtApiLogin.Areas.Identity.Data;
using WdtApiLogin.Models;
using WdtApiLogin.Repo;

namespace WdtApiLogin.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiService _apiService;
        private readonly SignInManager<WdtApiLoginUser> _signInManager;

        public HomeController(
            IApiService apiService,
            SignInManager<WdtApiLoginUser> signInManager)
        {
            _apiService = apiService;
            _signInManager = signInManager;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [AllowAnonymous]
        public async Task<IActionResult> Faq()
        {
            var faqList = await _apiService.Faq.FindAllAsync(faq => string.IsNullOrWhiteSpace(faq.AccessName));

            if (_signInManager.IsSignedIn(User))
            {
                var fq2 = await _apiService.Faq.FindAllAsync(faq => !string.IsNullOrWhiteSpace(faq.AccessName));
                var joinedList = faqList.Concat(fq2.Where(faq => User.IsInRole(faq.AccessName)));
                return View(joinedList);
            }

            return View(faqList);
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
    }
}