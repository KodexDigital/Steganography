using Anaconda.UserViewResponse;
using Steganography.ViewModels;

namespace Steganography.Services
{
    public interface IAccountService
    {
        Task<ResponseHandler> RegisterUserAsync(string email);
        Task<ResponseHandler> LoginAsync(string email);
        Task<ResponseHandler> UserLoginAsync(LoginViewModel model);
        Task<ResponseHandler> LogoutAsync();
        Task<ResponseHandler> VerifyUserAsync(string verificationToken);
        Task<ResponseHandler> UserAccountValidationAsync(string token);
    }
}