using Microsoft.AspNetCore.Mvc;

namespace Platec.Controllers
{
    public class LogInController : Controller
    {
        // SHOW LOGIN PAGE
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // HANDLE LOGIN
        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            // SAMPLE LOGIN CHECK (replace with DB later)
            if (Username == "123" && Password == "123")
            {
                // Redirect to Home/Index view
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }
    }
}
