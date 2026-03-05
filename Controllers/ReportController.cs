using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platec.Data;
using Platec.Models;

namespace Platec.Controllers
{
    public class ReportController : Controller
    {
        private readonly MyAppContext _context;

        public ReportController(MyAppContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime? date, int? courseId)
        {
            DateTime selectedDate = date ?? DateTime.Today;
            
            // Get user role and ID from session
            var role = HttpContext.Session.GetString("UserRole");
            var userIdString = HttpContext.Session.GetString("UserId");
            int userId = string.IsNullOrEmpty(userIdString) ? 0 : int.Parse(userIdString);

            // Get courses based on role
            List<Course> availableCourses;
            if (role == "Teacher")
            {
                // Only courses assigned to this teacher
                availableCourses = _context.Courses
                    .Include(c => c.Teacher)
                    .Where(c => c.TeacherId == userId)
                    .ToList();
            }
            else
            {
                // Admin sees all courses
                availableCourses = _context.Courses
                    .Include(c => c.Teacher)
                    .ToList();
            }

            // Get student IDs from filtered courses - students have CourseId property
            var courseIds = availableCourses.Select(c => c.CourseId).ToList();
            var studentIds = _context.Students
                .Where(s => courseIds.Contains(s.CourseId))
                .Select(s => s.ID)
                .ToList();

            // Debug: Check what we have
            ViewBag.DebugInfo = $"Role: {role}, UserId: {userId}, Available Courses: {availableCourses.Count}, Student IDs: [{string.Join(", ", studentIds)}]";

            // Get daily data filtered by courses
            var dailyAttendance = _context.ClassStatuses
                .Include(a => a.Student)
                .Where(a => a.Date.Date == selectedDate.Date && studentIds.Contains(a.StudentId))
                .ToList();

            // Debug: Check all attendance for the date
            var allDateAttendance = _context.ClassStatuses
                .Include(a => a.Student)
                .Where(a => a.Date.Date == selectedDate.Date)
                .ToList();

            ViewBag.DebugInfo += $", All attendance for {selectedDate:yyyy-MM-dd}: {allDateAttendance.Count}, Filtered: {dailyAttendance.Count}";

            // If no filtered data, show what we have
            if (!dailyAttendance.Any() && allDateAttendance.Any())
            {
                ViewBag.DebugInfo += " - FILTERING REMOVED ALL RECORDS!";
                // Show what student IDs are in attendance vs courses
                var attendanceStudentIds = allDateAttendance.Select(a => a.StudentId).ToList();
                ViewBag.DebugInfo += $" Attendance StudentIds: [{string.Join(", ", attendanceStudentIds)}]";
            }

            // Apply course filter if specified (for both admin and teacher)
            if (courseId.HasValue)
            {
                var selectedCourseStudentIds = _context.Students
                    .Where(s => s.CourseId == courseId.Value)
                    .Select(s => s.ID)
                    .ToList();
                dailyAttendance = dailyAttendance.Where(a => selectedCourseStudentIds.Contains(a.StudentId)).ToList();
            }

            ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");
            ViewBag.PresentCount = dailyAttendance.Count(a => a.Status == "Present");
            ViewBag.AbsentCount = dailyAttendance.Count(a => a.Status == "Absent");
            ViewBag.LateCount = dailyAttendance.Count(a => a.Status == "Late");
            
            // Pass courses and selected course to view
            ViewBag.AvailableCourses = availableCourses;
            ViewBag.SelectedCourseId = courseId;

            // Get weekly data (last 7 days including today) - filtered by courses and selected course
            var startOfWeek = selectedDate.AddDays(-6);
            var weekDates = Enumerable.Range(0, 7)
                                      .Select(i => startOfWeek.AddDays(i))
                                      .ToList();

            ViewBag.WeekLabels = weekDates.Select(d => d.ToString("MM/dd")).ToList();

            // Apply course filtering to weekly data as well
            List<int> filteredStudentIds;
            if (courseId.HasValue)
            {
                filteredStudentIds = _context.Students
                    .Where(s => s.CourseId == courseId.Value)
                    .Select(s => s.ID)
                    .ToList();
            }
            else
            {
                filteredStudentIds = studentIds;
            }

            ViewBag.WeekPresent = weekDates
                .Select(d => _context.ClassStatuses.Count(a => a.Date.Date == d && a.Status == "Present" && filteredStudentIds.Contains(a.StudentId)))
                .ToList();

            ViewBag.WeekAbsent = weekDates
                .Select(d => _context.ClassStatuses.Count(a => a.Date.Date == d && a.Status == "Absent" && filteredStudentIds.Contains(a.StudentId)))
                .ToList();

            ViewBag.WeekLate = weekDates
                .Select(d => _context.ClassStatuses.Count(a => a.Date.Date == d && a.Status == "Late" && filteredStudentIds.Contains(a.StudentId)))
                .ToList();

            return View(dailyAttendance);
        }

    }
}
