using Microsoft.AspNetCore.Mvc;
using Platec.Data;
using Platec.Models;

namespace Platec.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly MyAppContext _context;

        public RegistrationController(MyAppContext context)
        {
            _context = context;
        }

        // SHOW LOGIN PAGE
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddUser(string Username, string Password, string Role)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Role))
            {
                ViewBag.Message = "Please fill in all fields!";
                return View("Index"); // show the same page
            }

            // Convert string to enum
            UserRole userRole;
            if (!Enum.TryParse(Role, out userRole))
            {
                ViewBag.Message = "Invalid role selected!";
                return View("Index");
            }

            // Create new user
            var newUser = new Users
            {
                Username = Username,
                Password = Password, // hash in production
                Role = userRole
            };

            _context.User.Add(newUser);
            _context.SaveChanges();

            ViewBag.Message = "User added successfully!";

            ModelState.Clear(); // <-- This clears all the form fields
            return View("Index"); // stay on the same page
        }
    }
}
