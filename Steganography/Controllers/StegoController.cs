using Microsoft.AspNetCore.Mvc;
using Steganography.Helpers;
using Steganography.Services;
using Steganography.ViewModels;
using System.Drawing;
using System.Drawing.Imaging;

namespace Steganography.Controllers
{
    public class StegoController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly SteganographyService _stegService;

        public StegoController(IWebHostEnvironment env)
        {
            _env = env;
            _stegService = new SteganographyService();
        }

        [HttpPost]
        public async Task<IActionResult> Encode(StegoViewModel model)
        {
            if (model.ImageFile == null || string.IsNullOrWhiteSpace(model.Message))
            {
                TempData["Error"] = "Please upload an image and enter a message.";
                return RedirectToAction("Index");
            }

            try
            {
                string path = Path.Combine(_env.WebRootPath, "uploads", model.ImageFile.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                using (var bmp = new Bitmap(path))
                {
                    var encryptedMessage = CryptoHelper.Encrypt(model.Message, model.Password);
                    var encodedImage = _stegService.EmbedMessage(bmp, encryptedMessage);

                    if (!_stegService.CanEmbedMessage(bmp, encryptedMessage))
                    {
                        TempData["Error"] = $"Image too small. Choose a larger image or reduce message length.";
                        return RedirectToAction("Index");
                    }

                    string outPath = Path.Combine(_env.WebRootPath, "uploads", "stego_" + model.ImageFile.FileName);
                    encodedImage.Save(outPath, ImageFormat.Png);

                    ViewBag.EncodedImagePath = "/uploads/stego_" + model.ImageFile.FileName;
                    TempData["Success"] = "Message successfully embedded!";
                    LogHelper.Log("Encode", model.Message, "Success", "kodex", null);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to encode message: " + ex.Message;
                LogHelper.Log("Encode", model.Message, "Failed: " + ex.Message, "kodex", null);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Decode(IFormFile stegoImage, string password)
        {
            if (stegoImage == null)
            {
                TempData["Error"] = "Please upload a stego-image.";
                return RedirectToAction("Index");
            }

            try
            {
                string path = Path.Combine(_env.WebRootPath, "uploads", stegoImage.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await stegoImage.CopyToAsync(stream);
                }

                using (var bmp = new Bitmap(path))
                {
                    var extracted = _stegService.ExtractMessage(bmp);
                    var decrypted = CryptoHelper.Decrypt(extracted, password);
                    var model = new StegoViewModel { ExtractedMessage = decrypted };
                    TempData["Success"] = "Message successfully extracted!";
                    LogHelper.Log("Decode", model.Message, "Success", "kodex", null);
                    return View("Index", model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to extract message: " + ex.Message;
                LogHelper.Log("Decode", "model.Message", "Failed: " + ex.Message, "kodex", null);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Index() => View();
        public IActionResult Stats()
        {
            var username = User.Identity?.Name ?? "Kodex";
            var logs = LogHelper.GetLogs()
                .Where(l => l.Contains($"User: {username}"))
                .ToList();

            var encodeCount = logs.Count(l => l.Contains("| Encode |"));
            var decodeCount = logs.Count(l => l.Contains("| Decode |"));

            var lastLog = logs.LastOrDefault();

            var lastAction = lastLog?.Split('|')[1].Trim();
            DateTime? lastTime = DateTime.TryParse(lastLog?.Split('|')[0].Trim(), out var parsed) ? parsed : null;

            var model = new UserStatsViewModel
            {
                Username = username,
                TotalActions = logs.Count,
                EncodeCount = encodeCount,
                DecodeCount = decodeCount,
                LastAction = lastAction ?? "N/A",
                LastActionTime = lastTime
            };

            return View(model);
        }
    }

}
