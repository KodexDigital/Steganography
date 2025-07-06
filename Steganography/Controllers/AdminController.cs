using Anaconda.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Steganography.Services;

namespace Steganography.Controllers
{
    public class AdminController(IActivityLoggerService activityLoggerService) : Controller
    {
        protected readonly IActivityLoggerService _activityLoggerService = activityLoggerService;
        public IActionResult Index() => View();
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

        [HttpGet("visits")]
        public async Task<IActionResult> VisitorLogs()
        {
            var visits = await _activityLoggerService.GetVisitationInfosAsync();
            return View(visits);
        }
    }
}
