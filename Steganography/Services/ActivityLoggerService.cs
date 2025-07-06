using Anaconda.Models;
using Newtonsoft.Json;
using Steganography.Models;

namespace Steganography.Services
{
    public class ActivityLoggerService(IHttpContextAccessor httpContextAccessor)
    {
        protected readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
        public async Task LogUserActivityAsync()
        {
			try
			{
                var ip = GetIpAddress(HttpContext);
                var userAgent = GetUserAgent(HttpContext);
                var clientInfo = GetClientInfo(HttpContext);
                var geoJson = await GetLocationAsync(ip);
                var location = JsonConvert.DeserializeObject<GeoLocation>(geoJson);

                var visitor = new VisitationInfo
                {
                    IpAddress = ip,
                    Browser = clientInfo.Browser,
                    OperatingSystem = clientInfo.OS,
                    Device = clientInfo.Device,
                    Location = location
                };
            }
			catch (Exception)
			{
				throw;
			}
        }
    }
}
