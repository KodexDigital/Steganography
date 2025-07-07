using Anaconda.Models;
using Anaconda.UserViewResponse;
using Anaconda.UserViewResponse.ViewResponses;
using Steganography.ViewModels;

namespace Steganography.Services
{
    public interface ISteganographyService
    {
        Task<ResponseHandler<EncryptMessageResponse>> EncryptMessageAsync(EncodeViewModel model, Guid userId);
        Task<ResponseHandler<StegOutViewModel>> DecodeMessageAsync(DecodeViewModel model, Guid userId);
        Task<IEnumerable<StegStatelessFile>> GetAllStegFilesAsync(Guid userId);
        Task<StegStatelessFile> GetStegFileAsync(Guid id);
        Task<ResponseHandler> DeleteStegFileAsync(Guid id);
    }
}