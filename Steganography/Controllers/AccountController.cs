using Microsoft.AspNetCore.Mvc;
using Steganography.Services;
using System.Threading.Tasks;

namespace Steganography.Controllers
{
    public class AccountController(IAccountService accountService) : UserBaseController
    {
        protected readonly IAccountService accountService = accountService;

        [HttpPost("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("verify")]
        public async Task<IActionResult> VerifyUser(string token)
        {
            var verificationResult = await accountService.UserAccountValidationAsync(token);
            if(!verificationResult.Status)
            {
                ViewBag.ErrorMessage = verificationResult.Message;
                return View("Error");
            }
            return RedirectToAction("StegIn", "Stego");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
