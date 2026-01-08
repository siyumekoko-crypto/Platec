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
            return View();
        }
    }
}
