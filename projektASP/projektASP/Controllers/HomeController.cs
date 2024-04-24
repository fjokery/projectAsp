using Microsoft.AspNetCore.Mvc;
using projektASP.Models;
using System.Diagnostics;
using System.Web;

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

        public IActionResult hej()
        {
            return View();
        }

        public IActionResult Login()
        {

            return View();
        }

		public IActionResult redirect()
		{

			return View();
		}

		public void Logout()
        {
            SQlite.loggedInUser = null;
            Response.Redirect("Index");
        }


        public void OnLogin()
        {
            if (Request.Method == "POST")
            {
                string username = Request.Form["usernameInput"];
                string password = Request.Form["passwordInput"];

                bool isAuthenticated = SQlite.Login(username, password);
                if (isAuthenticated)
                {
                    //Saves username cookie
                    Response.Cookies.Append("Username", username);
                    // Redirect user to dashboard or another page upon successful login
                    Response.Redirect("redirect");
                }
                else
                {
                    // Handle failed login attempt (e.g., display error message)
                    Console.WriteLine("lol no");
                    Response.Redirect("Login");
                }
            }
        }

        public void OnRegister()
        {
            if(Request.Method == "POST")
            {
                string username = Request.Form["regUsernameInput"];
                string email = Request.Form["regEmailInput"];
                string password = Request.Form["regPasswordInput"];

                bool isAuthenticated = SQlite.Register(username, email, password);

                if (isAuthenticated)
                {
                    Response.Redirect("Index");
                }
                else
                {
                    Console.WriteLine("Something went wrong");
                    Response.Redirect("Login");
                }

            }
        }

        public string GetUsername()
        {
            return Request.Cookies["Username"];
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
