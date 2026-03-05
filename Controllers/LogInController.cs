using Microsoft.AspNetCore.Mvc;
using Platec.Data;
using Platec.Models;

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

        // //HANDLE LOGIN
        //[HttpPost]
        // public IActionResult Login(string Email, string Password)
        // {
        //     var user = _context.User
        //         .FirstOrDefault(u => u.Email == Email && u.Password == Password);

        //     if (user != null)
        //     {
        //         // login success
        //         return RedirectToAction("Index", "Admin");
        //     }

        //     ViewBag.Error = "Invalid username or password";
        //     return View("Index");
        // }

        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            var user = _context.User
                .FirstOrDefault(u => u.Email == Email && u.Password == Password);

            if (user != null)
            {
                // Store info in session
                HttpContext.Session.SetString("UserRole", ((int)user.Role).ToString());
                HttpContext.Session.SetString("UserId", user.ID.ToString());
                HttpContext.Session.SetString("UserRole", user.Role.ToString()); // Teacher/Admin

                // Redirect based on role
                if (user.Role == UserRole.Teacher)
                    return RedirectToAction("Index", "Class"); // teacher dashboard
                else
                    return RedirectToAction("Index", "Admin"); // admin dashboard
            }

            ViewBag.Error = "Invalid username or password";
            return View("Index");
        }

    }
}
