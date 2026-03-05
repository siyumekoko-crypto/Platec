using Microsoft.AspNetCore.Mvc;
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
        [HttpGet]
        public IActionResult Index()
        {
            // Get counts of attendance status for all students today
            var today = DateTime.Today;

            var attendanceCounts = _context.ClassStatuses
                .Where(a => a.Date.Date == today)
                .GroupBy(a => a.Status) // Status = "Present", "Absent", "Late"
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Prepare default values in case a status has 0
            int present = attendanceCounts.FirstOrDefault(x => x.Status == "Present")?.Count ?? 0;
            int absent = attendanceCounts.FirstOrDefault(x => x.Status == "Absent")?.Count ?? 0;
            int late = attendanceCounts.FirstOrDefault(x => x.Status == "Late")?.Count ?? 0;

            ViewBag.Present = present;
            ViewBag.Absent = absent;
            ViewBag.Late = late;

            return View();
        }
    }
}
