﻿using Anaconda.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Steganography.Services;
using Steganography.ViewModels;
using System.Security.Claims;

namespace Steganography.Controllers
{
    [Authorize]
    public class StegoController(ISteganographyService service) : UserBaseController
    {
        private readonly ISteganographyService _service = service;

        [HttpGet("steg-in")]
        public IActionResult StegIn() => View();

        [HttpPost("steg-in")]
        public async Task<IActionResult> StegIn(StegInViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var response = await _service.EncryptMessageAsync(new EncodeViewModel
            {
                File = model.Image,
                SecretMessage = model.Message,
                StegPassKey = model.StegKey
            }, AuthenticateUser());

            if (response.Status)
            {
                TempData["SuccessMessage"] = response.Message;
            }
            else
            {
                TempData["ErrorMessage"] = response.Message;
            }

            return RedirectToAction(nameof(StegIn));
        }

        [HttpGet("steg-out")]
        public IActionResult StegOut() => View();

        [HttpPost("steg-out")]
        public async Task<IActionResult> StegOut(DecodeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var response = await _service.DecodeMessageAsync(model, AuthenticateUser());
            if (response.Status)
            {
                TempData["SuccessMessage"] = response.Message;
                model.ExtractedMessage = response.Data?.ExtractedMessage;
                return View(model);
            }
            else
            {
                TempData["ErrorMessage"] = response.Message;
            }

            return RedirectToAction(nameof(StegOut));
        }

        [HttpGet("steg-images")]
        public async Task<IActionResult> StegImages()
        {
            var stegImages = await _service.GetAllStegFilesAsync(AuthenticateUser().UserId);
            return View(stegImages);
        }

        [HttpPost("delete-steg-file")]
        public async Task<IActionResult> DeleteStegImage(Guid id)
        {
            var response = await _service.DeleteStegFileAsync(id);
            if (!response.Status)
            {
                TempData["ErrorMessage"] = "Deleting file failed";
                return RedirectToAction(nameof(StegImages));
            }

            TempData["SuccessMessage"] = response.Message;
            return RedirectToAction(nameof(StegImages));
        }

        [HttpGet("stats")]
        public IActionResult Stats()
        {
            var username = User.Identity?.Name;
            var logs = LogHelper.GetLogs()
                .Where(l => l.Contains($"| User: {username} |"))
                .ToList();

            var encodeCount = logs.Count(l => l.Contains("| Encode |"));
            var encodeSuccessCount = logs.Count(l => l.Contains("| Encode | Success |"));
            var encodeFailedCount = logs.Count(l => l.Contains("| Encode | Failed |"));

            var decodeCount = logs.Count(l => l.Contains("| Decode |"));
            var decodeSuccessCount = logs.Count(l => l.Contains("| Decode | Success |"));
            var decodeFailedCount = logs.Count(l => l.Contains("| Decode | Failed |"));

            var lastLog = logs.LastOrDefault();

            var lastAction = lastLog?.Split('|')[1].Trim();
            DateTime? lastTime = DateTime.TryParse(lastLog?.Split('|')[0].Trim(), out var parsed) ? parsed : null;

            var model = new UserStatsViewModel
            {
                Username = username,
                TotalActions = logs.Count,
                EncodeCount = encodeCount,
                EncodeSuccessCount = encodeSuccessCount,
                EncodeFailedCount = encodeFailedCount,
                DecodeCount = decodeCount,
                DecodeSuccessCount = decodeSuccessCount,
                DecodeFailedCount = decodeFailedCount,
                LastAction = lastAction ?? "N/A",
                LastActionTime = lastTime
            };

            return View(model);
        }
        private (Guid UserId, string UserName) AuthenticateUser()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var userName = User.Identity.Name!;
                return (userId, userName);
            }
            return (Guid.Empty, string.Empty);
        }
    }
}