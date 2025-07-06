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

        //[Authorize(Roles = "Admin")]
        [HttpGet("logs")]
        public IActionResult AllLogs()
        {
            var allLogs = LogHelper.GetLogs()
                                .OrderByDescending(l => l)
                                .ToArray();

            return View(allLogs);
        }

        [HttpGet("visits")]
        public async Task<IActionResult> VisitorLogs()
        {
            var visits = await _activityLoggerService.GetVisitationInfosAsync();
            return View(visits);
        }
    }
}
