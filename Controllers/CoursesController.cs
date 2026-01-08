using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        // GET: Courses Dashboard
        [HttpGet]
        public IActionResult Index()
        {
            // Load all courses with teacher info
            var courses = _context.Courses
                .Include(c => c.Teacher)  // load Teacher navigation property
                .ToList();

            return View(courses);
        }

        [HttpGet]
        public IActionResult AddCourses()
        {
            return View();
        }

        [HttpGet, HttpPost]
        public IActionResult AddCourse(string CourseName)
        {
            if (Request.Method == "POST")
            {
                if (string.IsNullOrWhiteSpace(CourseName))
                {
                    ViewBag.Message = "Course name is required!";
                    return View();
                }

                var course = new Course
                {
                    CourseName = CourseName
                };

                _context.Courses.Add(course);
                _context.SaveChanges();

                ViewBag.Message = "Course added successfully!";
                ModelState.Clear();
            }

            return View("AddCourses");
        }

        //// GET: Show Add Course Page
        //[HttpGet]
        //public IActionResult AddCourse()
        //{
        //    return View();
        //}

        //// POST: Save Course
        //[HttpPost]
        //public IActionResult AddCourse(string CourseName)
        //{
        //    if (string.IsNullOrWhiteSpace(CourseName))
        //    {
        //        ViewBag.Message = "Course name is required!";
        //        return View();
        //    }

        //    // Create new course object
        //    var course = new Course
        //    {
        //        CourseName = CourseName
        //    };

        //    // Add to DB and save
        //    _context.Courses.Add(course);
        //    _context.SaveChanges();

        //    ViewBag.Message = "Course added successfully!";
        //    ModelState.Clear(); // clear form fields

        //    return View();
        //}
    }
}
