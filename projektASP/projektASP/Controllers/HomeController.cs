using Microsoft.AspNetCore.Mvc;
using projektASP.Models;
using System.Diagnostics;

namespace projektASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string userIP = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (!string.IsNullOrWhiteSpace(userIP))
            {
                SQlite.AddVisitor(userIP);
            }
            

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Forum()
        {
            return View();
        }

        public IActionResult Login()
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
