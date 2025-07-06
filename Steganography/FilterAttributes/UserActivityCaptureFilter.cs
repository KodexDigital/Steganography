using Microsoft.AspNetCore.Mvc.Filters;
using Steganography.Services;

namespace Steganography.FilterAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class UserActivityCaptureFilter(IActivityLoggerService activityLoggerService) : Attribute, IAsyncActionFilter
    {
        protected readonly IActivityLoggerService _activityLoggerService = activityLoggerService;
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context is not null)
            {
                await _activityLoggerService.LogUserActivityAsync();
            }

            await next();
        }
    }
}