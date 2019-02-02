using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WdtApiLogin.Controllers
{
    public class ErrorStatusController : Controller
    {
        [AllowAnonymous]
        [Route("/ErrorStatus/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            return View(statusCode);
        }
    }
}