using Anaconda.Models;
using Anaconda.UserViewResponse;
using Anaconda.UserViewResponse.ViewResponses;
using Steganography.ViewModels;

namespace Steganography.Services
{
    public interface ISteganographyService
    {
        Task<ResponseHandler<EncryptMessageResponse>> EncryptMessageAsync(EncodeViewModel model, (Guid userId, string username) user);
        Task<ResponseHandler<StegOutViewModel>> DecodeMessageAsync(DecodeViewModel model, (Guid userId, string username) user);
        Task<IEnumerable<StegStatelessFile>> GetAllStegFilesAsync(Guid userId);
        Task<StegStatelessFile> GetStegFileAsync(Guid id);
        Task<ResponseHandler> DeleteStegFileAsync(Guid id);
    }
}