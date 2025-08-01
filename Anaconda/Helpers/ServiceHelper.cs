﻿using Anaconda.UserViewResponse.ServiceResponses;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using UAParser;

namespace Anaconda.Helpers
{
    public class ServiceHelper
    {
        public static string GetIpAddress(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            if (ipAddress == "::1") ipAddress = "127.0.0.1";
            // If behind a reverse proxy, check headers : Check for reverse proxy headers (e.g., Cloudflare, nginx)
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out Microsoft.Extensions.Primitives.StringValues value))
            {
                var headerIp = value.FirstOrDefault();
                if (!string.IsNullOrEmpty(headerIp))
                    ipAddress = headerIp;
            }

            return ipAddress!;
        }
        public static string GetUserAgent(HttpContext context) => context.Request.Headers["User-Agent"]!;

        public static (string Browser, string OS, string Device) GetUserInfo(HttpContext context)
        {
            var uaParser = Parser.GetDefault();
            ClientInfo client = uaParser.Parse(context.Request.Headers["User-Agent"]);
            return (
                client.UA.ToString(),        // Browser
                client.OS.ToString(),        // Operating System
                client.Device.ToString().ToLower().Contains("other") ? "Desktop" : client.Device.ToString() // Device
            );
        }
        public static async Task<UserGeolocationResponse> GetUserLocationAsync(string ip)
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync($"http://ip-api.com/json/{ip}");
            return JsonConvert.DeserializeObject<UserGeolocationResponse>(response)!;
        }
    }
}