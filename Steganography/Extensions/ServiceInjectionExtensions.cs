using Anaconda.Settings;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Steganography.FilterAttributes;
using Steganography.Services;

namespace Steganography.Extensions
{
    public static class ServiceInjectionExtensions
    {
        public static void RegisterService(WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<ISteganographyService, SteganographyService>();
            builder.Services.AddTransient<IActivityLoggerService, ActivityLoggerService>();
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddTransient<IAccountService, AccountService>();
            builder.Services.AddTransient<IAdminViewService, AdminViewService>();
            builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

            builder.Services.Configure<SystemSettings>(builder.Configuration.GetSection(nameof(SystemSettings)));
            builder.Services.TryAddScoped<UserActivityCaptureFilter>();
        }
    }
}