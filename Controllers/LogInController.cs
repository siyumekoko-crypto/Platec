using Microsoft.AspNetCore.Mvc;
using Platec.Data;

namespace Platec.Controllers
{
    public class LogInController : Controller
    {
        private readonly MyAppContext _context;

        public LogInController(MyAppContext context)
        {
            _context = context;
        }

        // SHOW LOGIN PAGE
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // HANDLE LOGIN
        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            var user = _context.User
                .FirstOrDefault(u => u.Email == Username && u.Password == Password);

            if (user != null)
            {
                // login success
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }
    }
}
