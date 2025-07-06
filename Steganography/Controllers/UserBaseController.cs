using Microsoft.AspNetCore.Mvc;
using Steganography.FilterAttributes;

namespace Steganography.Controllers
{
    [ServiceFilter(typeof(UserActivityCaptureFilter))]
    public class UserBaseController : Controller
    {}
}