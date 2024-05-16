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

        public IActionResult Register()
        {

            return View();
        }

        public IActionResult redirect()
		{

			return View();
		}

		public IActionResult Search()
		{

			return View();
		}

		//Resettar anv�ndarnamnscookien vilket loggar ut en
		public void Logout()
        {
            Response.Cookies.Append("Username", "");
            Response.Redirect("Index");
        }

        //Kollar om man kan logga in
        public void OnLogin()
        {
            if (Request.Method == "POST")
            {
                //Tar anv�ndarnamn och l�senord fr�n textrutor p� loginsidan
                string username = Request.Form["usernameInput"];
                string password = Request.Form["passwordInput"];

                bool isAuthenticated = SQlite.Login(username, password);
                if (isAuthenticated)
                {
                    //Sparar anv�ndarnamnet i en cookie
                    Response.Cookies.Append("Username", username);
                    //Skickar en till indexsidan
                    Response.Redirect("Index");
                }
                else
                {
                    //Om inloggningen misslyckades
                    Console.WriteLine("Fel anv�ndarnamn eller l�senord");
                    Response.Redirect("Login");
                }
            }
        }

        //Registrerar och loggar in en
        public void OnRegister()
        {
            if(Request.Method == "POST")
            {
				//Tar anv�ndarnamn mail och l�senord fr�n textrutor p� loginsidan
				string username = Request.Form["regUsernameInput"];
                string email = Request.Form["regEmailInput"];
                string password = Request.Form["regPasswordInput"];
                int avatar = Int32.Parse(Request.Form["regAvatarInput"]);

                bool isAuthenticated = SQlite.Register(username, email, password, avatar);

                if (isAuthenticated)
                {
                    SQlite.Login(username, password);
                    Response.Redirect("Index");
                    
                }
                else
                {
                    Console.WriteLine("Registrering misslyckades");
                    Response.Redirect("Login");
                }

            }
        }

        //Skapa post
        public void CreatePost()
        {
			if (Request.Method == "POST")
			{
                //Tar titel och text fr�n textrutor
				string title = Request.Form["title"];
				string text = Request.Form["text"];

				bool posted = SQlite.CreatePost(title, text, Request.Cookies["Username"]);

				if (posted)
				{
					Response.Redirect("Hej");
				}
				else
				{
					Console.WriteLine("Something went wrong");
					Response.Redirect("Index");
				}

			}
		}

        //Söka på forumet
        public void SearchPost()
        {
            if (Request.Method == "POST")
            {
                string search = Request.Form["search"];
                Response.Cookies.Append("Search", search);
                Response.Redirect("Search");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
