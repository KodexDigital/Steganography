using Anaconda.UserViewResponse;
using Steganography.ViewModels;

namespace Steganography.Services
{
    public interface IAdminViewService
    {
        Task<ResponseHandler<DashboardDataViewModel>> DashboardDataAsync();
        Task<ResponseHandler<IEnumerable<DashboardUserResponse>>> DashboardUsersAsync();
    }
}