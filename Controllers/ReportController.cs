using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platec.Data;

namespace Platec.Controllers
{
    public class ReportController : Controller
    {
        private readonly MyAppContext _context;

        public ReportController(MyAppContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime? date)
        {
            DateTime selectedDate = date ?? DateTime.Today;

            // Get daily data
            var dailyAttendance = _context.ClassStatuses
                .Include(a => a.Student)
                .Where(a => a.Date.Date == selectedDate.Date)
                .ToList();

            ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");
            ViewBag.PresentCount = dailyAttendance.Count(a => a.Status == "Present");
            ViewBag.AbsentCount = dailyAttendance.Count(a => a.Status == "Absent");
            ViewBag.LateCount = dailyAttendance.Count(a => a.Status == "Late");

            // Get weekly data (last 7 days including today)
            var startOfWeek = selectedDate.AddDays(-6);
            var weekDates = Enumerable.Range(0, 7)
                                      .Select(i => startOfWeek.AddDays(i))
                                      .ToList();

            ViewBag.WeekLabels = weekDates.Select(d => d.ToString("MM/dd")).ToList();

            ViewBag.WeekPresent = weekDates
                .Select(d => _context.ClassStatuses.Count(a => a.Date.Date == d && a.Status == "Present"))
                .ToList();

            ViewBag.WeekAbsent = weekDates
                .Select(d => _context.ClassStatuses.Count(a => a.Date.Date == d && a.Status == "Absent"))
                .ToList();

            ViewBag.WeekLate = weekDates
                .Select(d => _context.ClassStatuses.Count(a => a.Date.Date == d && a.Status == "Late"))
                .ToList();

            return View(dailyAttendance);
        }

    }
}
