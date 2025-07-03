using Anaconda.UserViewResponse;
using Anaconda.UserViewResponse.ViewResponses;
using Steganography.ViewModels;

namespace Steganography.Services
{
    public interface ISteganographyService
    {
        Task<ResponseHandler<EncryptMessageResponse>> EncryptMessageAsync(EncodeViewModel model);
        Task<ResponseHandler<StegOutViewModel>> DecodeMessageAsync(DecodeViewModel model);
    }
}