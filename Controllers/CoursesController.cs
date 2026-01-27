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
            var courses = _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Students)
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

        // GET: Show Course Details page (already exists)
        public IActionResult CoursesDetails(int id)
        {
            var course = _context.Courses
                    .Include(c => c.Teacher)
                    .Include(c => c.Students)
                    .FirstOrDefault(c => c.CourseId == id);

            if (course == null)
                return NotFound();

            // Prepare SelectList for dropdown
            ViewBag.TeacherList = new SelectList(
                _context.User.Where(u => u.Role == UserRole.Teacher).ToList(),
                "ID",
                "Email",
                course.TeacherId  // This preselects the current teacher
            );

            return View(course);
        }

        // POST: Set/Change teacher
        [HttpPost]
        public IActionResult SetTeacher(int CourseId, int TeacherId)
        {
            var course = _context.Courses.Find(CourseId);
            if (course == null)
                return NotFound();

            course.TeacherId = TeacherId;
            _context.SaveChanges();

            return RedirectToAction("CoursesDetails", new { id = CourseId });
        }


        public IActionResult Edit(int id)
        {
            var course = _context.Courses
                .Include(c => c.Teacher)
                .FirstOrDefault(c => c.CourseId == id);

            if (course == null)
                return NotFound();

            ViewBag.Teachers = _context.User
                .Where(u => u.Role == UserRole.Teacher)
                .ToList();

            return View(course);
        }

        [HttpPost]
        public IActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Update(course);
                _context.SaveChanges();
                return RedirectToAction("Details", new { id = course.CourseId });
            }

            ViewBag.Teachers = _context.User
                .Where(u => u.Role == UserRole.Teacher)
                .ToList();

            return View(course);
        }
    }
}
