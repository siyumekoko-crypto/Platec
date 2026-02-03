using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult StudentManagement()
        {
            ViewBag.Courses = new SelectList(
                _context.Courses,
                "CourseId",
                "CourseName"
            );

            return View();
        }

        [HttpPost]
        public IActionResult AddUser(string Email, string Password, string Role)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Role))
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
            var newUser = new User
            {
                Email = Email,
                Password = Password, // hash in production
                Role = userRole
            };

            _context.User.Add(newUser);
            _context.SaveChanges();

            ViewBag.Message = "User added successfully!";

            ModelState.Clear(); // <-- This clears all the form fields
            return View("Index"); // stay on the same page
        }

        [HttpPost]
        public IActionResult AddStudent(
    string FirstName,
    string MiddleName,
    string LastName,
    string Username,
    string Password,
    int CourseId)
        {
            if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Message = "Please fill in all required fields!";
                ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
                return View("StudentManagement");
            }

            bool exists = _context.Students.Any(s => s.Username == Username);
            if (exists)
            {
                ViewBag.Message = "Username already exists!";
                ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
                return View("StudentManagement");
            }

            var student = new Student
            {
                FirstName = FirstName,
                MiddleName = MiddleName,
                LastName = LastName,
                Username = Username,
                Password = Password, // ⚠ hash in production
                CourseId = CourseId
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            ViewBag.Message = "Student added successfully!";
            ModelState.Clear();

            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
            return View("StudentManagement");
        }

        //[HttpPost]
        //public IActionResult AddStudent(string Username, string Password, int CourseId)
        //{
        //    if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        //    {
        //        ViewBag.Message = "Please fill in all fields!";
        //        ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
        //        return View("StudentManagement");
        //    }

        //    bool exists = _context.Students.Any(s => s.Username == Username);
        //    if (exists)
        //    {
        //        ViewBag.Message = "Username already exists!";
        //        ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
        //        return View("StudentManagement");
        //    }

        //    var newStudent = new Student
        //    {
        //        Username = Username,
        //        Password = Password, // hash in production
        //        CourseId = CourseId   // ✅ SAVE COURSE
        //    };

        //    _context.Students.Add(newStudent);
        //    _context.SaveChanges();

        //    ViewBag.Message = "Student added successfully!";
        //    ModelState.Clear();

        //    ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
        //    return View("StudentManagement");
        //}
    }
}
