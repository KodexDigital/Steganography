using Anaconda.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Steganography.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Logs()
        {
            //log by user
           var username = User.Identity?.Name ?? "Kodex";
            var logs = LogHelper.GetLogs()
                                .Where(log => log.Contains($"User: {username}"))
                                .ToArray();

            //var logs = LogHelper.GetLogs();
            return View(logs);
        }

        //[Authorize(Roles = "Admin")]
        public IActionResult AllLogs()
        {
            var allLogs = LogHelper.GetLogs();
            return View("Logs", allLogs);
        }

        public IActionResult VisitorLogs()
        {
            //var logs = _context.VisitorLogs.OrderByDescending(l => l.VisitTime).ToList();
            return View();
        }
    }
}
