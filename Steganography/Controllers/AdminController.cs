using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Steganography.Helpers;

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

    }
}
