using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Platec.Data;
using Platec.Models;

namespace Platec.Controllers
{
    public class CoursesController : Controller
    {
        private readonly MyAppContext _context;

        public CoursesController(MyAppContext context)
        {
            _context = context;
        }

        // SHOW LOGIN PAGE
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddCourses()
        {
            return View();
        }

        // GET: Add Course
        [HttpGet]
        public IActionResult AddCourse()
        {
            // Get teachers for dropdown
            ViewBag.Teachers = new SelectList(_context.User.Where(u => u.Role == UserRole.Teacher), "Userid", "Username");
            return View();
        }

        // POST: Save Course
        [HttpPost]
        public IActionResult AddCourse(string CourseName, int? TeacherId)
        {
            if (string.IsNullOrWhiteSpace(CourseName))
            {
                ViewBag.Message = "Course name is required!";
                ViewBag.Teachers = new SelectList(_context.User.Where(u => u.Role == UserRole.Teacher), "Userid", "Username");
                return View();
            }

            var course = new Course
            {
                CourseName = CourseName,
                TeacherId = TeacherId
            };

            _context.Courses.Add(course);
            _context.SaveChanges();

            ViewBag.Message = "Course added successfully!";
            ModelState.Clear();

            ViewBag.Teachers = new SelectList(_context.User.Where(u => u.Role == UserRole.Teacher), "Userid", "Username");
            return View();
        }
    }
}
