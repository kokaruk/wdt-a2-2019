using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using WdtConsumer.Models;

namespace WdtConsumer.Controllers
{
    public class HomeController : Controller
    {
        // public IActionResult Index()
        // {
        //
        //     return View();
        // }

        public async Task<IActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync("http://kokaruk.com:59560/api/rooms");

                //var customers = JsonConvert.DeserializeObject<List<Room>>(result);
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
