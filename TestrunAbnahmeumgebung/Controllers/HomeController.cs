using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestrunAbnahmeumgebung.Models;

namespace TestrunAbnahmeumgebung.Controllers
{
    public class HomeController : Controller
    {
        private readonly TestDbContext _db;

        public HomeController(TestDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Insert a ping record to validate DB connectivity
                var ping = new Ping { Timestamp = DateTime.UtcNow, Message = "Ping from MVC" };
                _db.Pings.Add(ping);
                await _db.SaveChangesAsync();

                var pings = await _db.Pings.OrderByDescending(p => p.Id).Take(10).ToListAsync();
                return View(pings);
            }
            catch (Exception ex)
            {
                // Pass the exception message to the view for debugging (in Development only)
                ViewData["Error"] = ex.Message;
                return View(new List<Ping>());
            }
        }

        [HttpGet("/healthz")]
        public IActionResult Healthz()
        {
            return Ok(new { status = "Healthy" });
        }
    }
}
