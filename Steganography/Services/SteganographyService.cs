using Anaconda.Helpers;
using Anaconda.UserViewResponse;
using Anaconda.UserViewResponse.ViewResponses;
using Steganography.ViewModels;
using System.Drawing;
using System.Drawing.Imaging;

namespace Steganography.Services
{
    public class SteganographyService(IWebHostEnvironment env) : ISteganographyService
    {
        protected readonly IWebHostEnvironment _env = env;
        protected readonly SteganographyHelper _steganographyHelper = new();
        public async Task<ResponseHandler<EncryptMessageResponse>> EncryptMessageAsync(EncodeViewModel model)
        {
            var response = new ResponseHandler<EncryptMessageResponse>();
            try
            {
                if (model.File is null || model.File.Length.Equals(0)) throw new Exception("Please upload an image.");
                if (string.IsNullOrWhiteSpace(model.SecretMessage)) throw new Exception("Please supply the sacred message.");
                if (string.IsNullOrWhiteSpace(model.StegPassKey)) throw new Exception("Steg Key is required.");

                if (model.File.Length > 5 * 1024 * 1024) // 5 MB limit
                    throw new Exception("File size exceeds the maximum limit of 5 MB.");

                if (model.File.Length < 100) // Minimum size check
                    throw new Exception("File is too small to embed a message. Please choose a larger image.");

                if (model.File.ContentType is not "image/png" && model.File.ContentType is not "image/jpeg")
                    throw new Exception("Only PNG and JPEG images are supported.");

                string path = Path.Combine(_env.WebRootPath, "uploads/abc_stegs/", model.File.FileName.Replace(' ', '_'));
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                using var bmp = new Bitmap(path);
                var encryptedMessage = CryptoHelper.Encrypt(model.SecretMessage, model.StegPassKey);
                var encodedImage = SteganographyHelper.EmbedMessage(bmp, encryptedMessage);

                if (!SteganographyHelper.CanEmbedMessage(bmp, encryptedMessage))
                    throw new Exception("Image is too small to embed the message. Please choose a larger image or reduce message length.");

                string fileName = string.Concat(Path.GetFileNameWithoutExtension(model.File.FileName).Replace(' ', '_'), "_", Guid.NewGuid().ToString("N"));
                string fileExtension = Path.GetExtension(model.File.FileName);
                string name = string.Concat("_stg=>", fileName, fileExtension).ToLower();

                string outPath = Path.Combine(_env.WebRootPath, "uploads/xyz_stegs/", name);
                encodedImage.Save(outPath, ImageFormat.Png);

                response.Data = new EncryptMessageResponse(string.Concat("/uploads/xyz_stegs/", name));
                response.Message = "Message successfully embedded!";
                LogHelper.Log("Encode", response.Message, "Success", "kodex", null);
            }
            catch (Exception ex)
            {
                response.Message = "Failed to encode message";
                response.Status = false;
                LogHelper.Log("Encode", ex.Message, "Faild", "kodex", null);
            }

            return response;
        }

        public async Task<ResponseHandler<StegOutViewModel>> DecodeMessageAsync(DecodeViewModel model)
        {
            var response = new ResponseHandler<StegOutViewModel>();
            try
            {
                if (model.StegFile is null || model.StegFile.Length.Equals(0)) throw new Exception("Please upload the steged image.");
                if (string.IsNullOrWhiteSpace(model.StegPassKey)) throw new Exception("Enter the Steg Key.");

                string path = Path.Combine(_env.WebRootPath, "uploads/xyz_stegs/", model.StegFile.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.StegFile.CopyToAsync(stream);
                }

                using var bmp = new Bitmap(path);
                var extracted = SteganographyHelper.ExtractMessage(bmp);
                var decrypted = CryptoHelper.Decrypt(extracted, model.StegPassKey);

                response.Data = new StegOutViewModel(decrypted );
                response.Message = "Message successfully extracted!";
                LogHelper.Log("Decode", response.Message, "Success", "kodex", null);
            }
            catch (Exception ex)
            {
                response.Message = "Failed to extract message";
                response.Status = false;
                LogHelper.Log("Decode", ex.Message, "Faild", "kodex", null);
            }

            return response;
        }
    }
}