using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Platec.Data;
using Platec.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var role = HttpContext.Session.GetString("UserRole");
            var userIdString = HttpContext.Session.GetString("UserId");
            int userId = string.IsNullOrEmpty(userIdString) ? 0 : int.Parse(userIdString);

            List<Course> courses;

            if (role == "Teacher")
            {
                // Only courses assigned to this teacher
                courses = _context.Courses
                    .Include(c => c.Teacher)
                    .Include(c => c.Students)
                    .Where(c => c.TeacherId == userId)
                    .ToList();
            }
            else
            {
                // Admin sees all courses
                courses = _context.Courses
                    .Include(c => c.Teacher)
                    .Include(c => c.Students)
                    .ToList();
            }
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
        // GET: CoursesDetails
        //public IActionResult CoursesDetails(int id, DateTime? date) // <--- define date here
        //{
        //    var course = _context.Courses
        //            .Include(c => c.Teacher)
        //            .Include(c => c.Students)
        //            .FirstOrDefault(c => c.CourseId == id);

        //    if (course == null)
        //        return NotFound();

        //    // Prepare teacher dropdown
        //    ViewBag.TeacherList = new SelectList(
        //        _context.User.Where(u => u.Role == UserRole.Teacher).ToList(),
        //        "ID",
        //        "Email",
        //        course.TeacherId
        //    );

        //    //// Use selected date or today
        //    //DateTime selectedDate = date ?? DateTime.Today;
        //    //ViewBag.SelectedDate = selectedDate;

        //    //// Load attendance for that specific date
        //    //ViewBag.TodaysAttendance = _context.ClassStatuses
        //    //    .Where(s => s.Date.Date == selectedDate.Date)
        //    //    .ToList();

        //    DateTime selectedDate = date ?? DateTime.Today;
        //    var todaysAttendance = _context.ClassStatuses
        //        .Where(a => a.Date.Date == selectedDate.Date
        //                 && course.Students.Select(s => s.ID).Contains(a.StudentId))
        //        .ToList();


        //    ViewBag.TodaysAttendance = todaysAttendance;
        //    ViewBag.AttendanceDate = selectedDate.ToString("yyyy-MM-dd");

        //    return View(course);
        //}

        public IActionResult CoursesDetails(int id, DateTime? date)
        {
            var course = _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Students)
                .FirstOrDefault(c => c.CourseId == id);

            if (course == null)
                return NotFound();

            ViewBag.TeacherList = new SelectList(
                _context.User.Where(u => u.Role == UserRole.Teacher).ToList(),
                "ID",
                "Email",
                course.TeacherId
            );

            DateTime selectedDate = date ?? DateTime.Today;

            var todaysAttendance = _context.ClassStatuses
                .Where(a => a.Date.Date == selectedDate.Date &&
                       course.Students.Select(s => s.ID).Contains(a.StudentId))
                .ToList();

            ViewBag.TodaysAttendance = todaysAttendance;
            ViewBag.AttendanceDate = selectedDate.ToString("yyyy-MM-dd");

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

        [HttpPost]
        public IActionResult SaveAttendance(int CourseId, DateTime AttendanceDate,
        List<int> StudentIds, List<string> Statuses)
        {
            for (int i = 0; i < StudentIds.Count; i++)
            {
                // Check if record already exists for this student + course + date
                var existingStatus = _context.ClassStatuses
                    .FirstOrDefault(s => s.StudentId == StudentIds[i]
                                      && s.Date.Date == AttendanceDate.Date);

                if (existingStatus != null)
                {
                    // Update existing record
                    existingStatus.Status = Statuses[i];
                    _context.ClassStatuses.Update(existingStatus);
                }
                else
                {
                    // Add new record
                    var status = new ClassStatus
                    {
                        StudentId = StudentIds[i],
                        Date = AttendanceDate,
                        Status = Statuses[i]
                    };
                    _context.ClassStatuses.Add(status);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("CoursesDetails", new { id = CourseId, date = AttendanceDate });
        }


    }
}
