using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platec.Data;

namespace Platec.Controllers
{
    public class AdminController : Controller
    {
        private readonly MyAppContext _context;

        public AdminController(MyAppContext context)
        {
            _context = context;
        }

        // SHOW LOGIN PAGE
        //[HttpGet]
        [HttpGet]
        public async Task<IActionResult> Index(int? courseId)
        {
            // 1️⃣ Get all courses for dropdown
            var courses = await _context.Courses
                .Select(c => new { c.CourseId, c.CourseName })
                .ToListAsync();
            ViewBag.Courses = courses;

            // Default selected course
            if (courseId == null && courses.Count > 0)
                courseId = courses[0].CourseId;

            ViewBag.SelectedCourseId = courseId;

            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Sunday as first day
            var endOfWeek = startOfWeek.AddDays(6);

            // 2️⃣ Today's Attendance filtered by course
            var todayCounts = await _context.ClassStatuses
                .Include(a => a.Student)
                .Where(a => a.Date.Date == today && a.Student.CourseId == courseId)
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            int presentToday = todayCounts.FirstOrDefault(x => x.Status == "Present")?.Count ?? 0;
            int absentToday = todayCounts.FirstOrDefault(x => x.Status == "Absent")?.Count ?? 0;
            int lateToday = todayCounts.FirstOrDefault(x => x.Status == "Late")?.Count ?? 0;

            // 3️⃣ Weekly Attendance filtered by course
            var weekCounts = await _context.ClassStatuses
                .Include(a => a.Student)
                .Where(a => a.Date.Date >= startOfWeek
                         && a.Date.Date <= endOfWeek
                         && a.Student.CourseId == courseId)
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            int presentWeek = weekCounts.FirstOrDefault(x => x.Status == "Present")?.Count ?? 0;
            int absentWeek = weekCounts.FirstOrDefault(x => x.Status == "Absent")?.Count ?? 0;
            int lateWeek = weekCounts.FirstOrDefault(x => x.Status == "Late")?.Count ?? 0;

            // Pass to ViewBag
            ViewBag.PresentToday = presentToday;
            ViewBag.AbsentToday = absentToday;
            ViewBag.LateToday = lateToday;

            ViewBag.PresentWeek = presentWeek;
            ViewBag.AbsentWeek = absentWeek;
            ViewBag.LateWeek = lateWeek;

            return View();
        }
        //public IActionResult Index()
        //{
        //    // Get counts of attendance status for all students today
        //    var today = DateTime.Today;

        //    var attendanceCounts = _context.ClassStatuses
        //        .Where(a => a.Date.Date == today)
        //        .GroupBy(a => a.Status) // Status = "Present", "Absent", "Late"
        //        .Select(g => new
        //        {
        //            Status = g.Key,
        //            Count = g.Count()
        //        })
        //        .ToList();

        //    // Prepare default values in case a status has 0
        //    int present = attendanceCounts.FirstOrDefault(x => x.Status == "Present")?.Count ?? 0;
        //    int absent = attendanceCounts.FirstOrDefault(x => x.Status == "Absent")?.Count ?? 0;
        //    int late = attendanceCounts.FirstOrDefault(x => x.Status == "Late")?.Count ?? 0;

        //    ViewBag.Present = present;
        //    ViewBag.Absent = absent;
        //    ViewBag.Late = late;

        //    return View();
        //}
    }
}
