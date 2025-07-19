using Anaconda.Enums;
using Anaconda.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Steganography.Services;
using Steganography.ViewModels;

namespace Steganography.Controllers
{
    public class AdminController(IActivityLoggerService activityLoggerService, IAccountService accountService, IAdminViewService adminViewService) : Controller
    {
        protected readonly IActivityLoggerService _activityLoggerService = activityLoggerService;
        protected readonly IAccountService accountService = accountService;
        protected readonly IAdminViewService adminViewService = adminViewService;

        public async Task<IActionResult> Index()
        {
            var dashboardData = new AdminViewModel();
            var flowControl = CheckUserStateOnAction();
            if (flowControl)
            {
                var data = await adminViewService.DashboardDataAsync();
                var users = await adminViewService.DashboardUsersAsync();

                dashboardData.Users = users.Data;
                dashboardData.DashboardData = data.Data;

                return View(dashboardData);
            }
            return RedirectToAccessDenied();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid request";
                return View(model);
            }

            var login = await accountService.UserLoginAsync(model);
            if (!login.Status)
            {
                TempData["ErrorMessage"] = login.Message;
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet("logs")]
        public IActionResult AllLogs()
        {
            var flowControl = CheckUserStateOnAction();
            if (flowControl)
            {
                var allLogs = LogHelper.GetLogs()
                                .OrderByDescending(l => l)
                                .ToArray();

                return View(allLogs);
            }
            return RedirectToAccessDenied();
        }

        [HttpGet("visits")]
        public async Task<IActionResult> VisitorLogs()
        {
            var flowControl = CheckUserStateOnAction();
            if (flowControl)
            {
                var visits = await _activityLoggerService.GetVisitationInfosAsync();
                return View(visits);
            }
            return RedirectToAccessDenied();
        }

        protected IActionResult RedirectToAccessDenied() => RedirectToAction("AccessDenied", "Account");
        protected bool CheckUserStateOnAction()
            => User.Identity!.IsAuthenticated && User.IsInRole(UserRole.Admin.ToString());
    }
}