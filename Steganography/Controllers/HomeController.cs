using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Steganography.ViewModels;

namespace Steganography.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : UserBaseController
    {
        private readonly ILogger<HomeController> _logger = logger;

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();
        public IActionResult Cookies() => View();
        public IActionResult Terms() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
