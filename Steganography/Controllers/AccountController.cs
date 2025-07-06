using Microsoft.AspNetCore.Mvc;

namespace Steganography.Controllers
{
    public class AccountController : UserBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
