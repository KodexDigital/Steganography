using Microsoft.AspNetCore.Mvc;

namespace Steganography.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
