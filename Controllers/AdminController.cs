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
        public async Task<IActionResult> Index()
        {

            // 1️⃣ Get the logged-in teacher's ID
            int teacherId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // 2️⃣ Get all course IDs assigned to this teacher
            var teacherCourseIds = await _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .Select(c => c.CourseId)
                .ToListAsync();

            // If no courses assigned, show 0 stats
            if (teacherCourseIds.Count == 0)
            {
                ViewBag.PresentToday = 0;
                ViewBag.AbsentToday = 0;
                ViewBag.LateToday = 0;
                ViewBag.PresentWeek = 0;
                ViewBag.AbsentWeek = 0;
                ViewBag.LateWeek = 0;
                return View();
            }

            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Sunday as first day
            var endOfWeek = startOfWeek.AddDays(6);

            // 3️⃣ Today's Attendance for all teacher's courses
            var todayCounts = await _context.ClassStatuses
                .Include(a => a.Student)
                .Where(a => teacherCourseIds.Contains(a.Student.CourseId)
                         && a.Date.Date == today)
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.PresentToday = todayCounts.FirstOrDefault(x => x.Status == "Present")?.Count ?? 0;
            ViewBag.AbsentToday = todayCounts.FirstOrDefault(x => x.Status == "Absent")?.Count ?? 0;
            ViewBag.LateToday = todayCounts.FirstOrDefault(x => x.Status == "Late")?.Count ?? 0;

            // 4️⃣ Weekly Attendance for all teacher's courses
            var weekCounts = await _context.ClassStatuses
                .Include(a => a.Student)
                .Where(a => teacherCourseIds.Contains(a.Student.CourseId)
                         && a.Date.Date >= startOfWeek
                         && a.Date.Date <= endOfWeek)
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.PresentWeek = weekCounts.FirstOrDefault(x => x.Status == "Present")?.Count ?? 0;
            ViewBag.AbsentWeek = weekCounts.FirstOrDefault(x => x.Status == "Absent")?.Count ?? 0;
            ViewBag.LateWeek = weekCounts.FirstOrDefault(x => x.Status == "Late")?.Count ?? 0;

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
