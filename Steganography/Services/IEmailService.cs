using Anaconda.Requests;
using Anaconda.UserViewResponse;

namespace Steganography.Services
{
    public interface IEmailService
    {
        Task<ResponseHandler> SendMailAsync(DefaultSendMailRequest request, string authHeader);
    }
}
