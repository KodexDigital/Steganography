using Microsoft.AspNetCore.Mvc;
using Steganography.Services;
using Steganography.ViewModels;
using System.Threading.Tasks;

namespace Steganography.Controllers
{
    public class AccountController(IAccountService accountService) : UserBaseController
    {
        protected readonly IAccountService accountService = accountService;

        [HttpPost("register")]
        public async Task<IActionResult> Register(AuthenticationViewModel model)
        {
            var registrationResult = await accountService.RegisterUserAsync(model.Email!);
            if(registrationResult.Status)
                TempData["SuccessMessage"] = "User registration successful. Please check your email (inbox/junk/spam folder) to verify account and gain access";
            else
                TempData["ErrorMessage"] = registrationResult.Message;

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthenticationViewModel model)
        {
            var loginResult = await accountService.LoginAsync(model.Email!);
            if (loginResult.Status)
                TempData["SuccessMessage"] = loginResult.Message;
            else
                TempData["ErrorMessage"] = loginResult.Message;

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("verify")]
        public async Task<IActionResult> VerifyUser(string token)
        {
            var verificationResult = await accountService.UserAccountValidationAsync(token);
            if(!verificationResult.Status)
            {
                TempData["ErrorMessage"] = verificationResult.Message;
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("StegIn", "Stego");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}