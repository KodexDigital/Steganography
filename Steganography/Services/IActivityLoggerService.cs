using Anaconda.Models;

namespace Steganography.Services
{
    public interface IActivityLoggerService
    {
        Task LogUserActivityAsync();
        Task<IEnumerable<VisitationInfo>> GetVisitationInfosAsync();
        Task<VisitationInfo> GetVisitationInfoAsync(Guid visitationId);
    }
}