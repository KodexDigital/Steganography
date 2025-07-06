using Anaconda.DataLayer;
using Anaconda.Helpers;
using Anaconda.Models;

namespace Steganography.Services
{
    public class ActivityLoggerService(IHttpContextAccessor httpContextAccessor, ServiceDbContext dbContext) : IActivityLoggerService
    {
        protected readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
        protected readonly ServiceDbContext dbContext = dbContext;
        public async Task LogUserActivityAsync()
        {
			try
			{
                var httpContext = httpContextAccessor.HttpContext;
                if(httpContext is not null)
                {
                    var ip = ServiceHelper.GetIpAddress(httpContext);
                    var userAgent = ServiceHelper.GetUserAgent(httpContext);
                    var (Browser, OS, Device) = ServiceHelper.GetUserInfo(httpContext);
                    var geoJson = await ServiceHelper.GetUserLocationAsync(ip);

                    var location = await SaveUserLocationAsync(geoJson);
                    await dbContext.VisitationInfos.AddAsync(new VisitationInfo
                    {
                        IpAddress = ip,
                        Browser = Browser,
                        OperatingSystem = OS,
                        Device = Device,
                        LocationId = location.Id,
                    });
                    await dbContext.SaveChangesAsync();
                }
            }
			catch (Exception)
			{
				throw;
			}
        }

        private async Task<GeoLocation> SaveUserLocationAsync(Anaconda.UserViewResponse.ServiceResponses.UserGeolocationResponse geoJson)
        {
            var location = await dbContext.GeoLocations.AddAsync(new GeoLocation
            {
                City = geoJson.City,
                Country = geoJson.Country,
                Isp = geoJson.ISP,
                RegionName = geoJson.RegionName,
                Query = geoJson.Query,
            });
            await dbContext.SaveChangesAsync();
            return location.Entity;
        }
    }
}
