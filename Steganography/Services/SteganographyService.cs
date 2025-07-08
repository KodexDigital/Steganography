using Anaconda.DataLayer;
using Anaconda.Enums;
using Anaconda.Helpers;
using Anaconda.Models;
using Anaconda.UserViewResponse;
using Anaconda.UserViewResponse.ViewResponses;
using Microsoft.EntityFrameworkCore;
using Steganography.ViewModels;
using System.Drawing;
using System.Drawing.Imaging;

namespace Steganography.Services
{
    public class SteganographyService(IWebHostEnvironment env, ServiceDbContext dbContext) : ISteganographyService
    {
        protected readonly IWebHostEnvironment _env = env;
        protected readonly ServiceDbContext dbContext = dbContext;        
        protected readonly SteganographyHelper _steganographyHelper = new();
        public async Task<ResponseHandler<EncryptMessageResponse>> EncryptMessageAsync(EncodeViewModel model, (Guid userId, string username) user)
        {
            var response = new ResponseHandler<EncryptMessageResponse>();
            string originalPath = string.Empty;
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

                originalPath = Path.Combine(_env.WebRootPath, "uploads/xyz_stegs/", model.File.FileName.Replace(' ', '_'));
                using (var stream = new FileStream(originalPath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                using var bmp = new Bitmap(originalPath);
                var encryptedMessage = CryptoHelper.Encrypt(model.SecretMessage, model.StegPassKey);
                var encodedImage = SteganographyHelper.EmbedMessage(bmp, encryptedMessage);

                if (!SteganographyHelper.CanEmbedMessage(bmp, encryptedMessage))
                    throw new Exception("Image is too small to embed the message. Please choose a larger image or reduce message length.");

                var userReference = HashHelper.ComputeHash(model.StegPassKey);
                var referenceKey = RandomHelper.GenerateRandomString(userReference.ToCharArray(), 5);
                string fileName = string.Concat(Path.GetFileNameWithoutExtension(model.File.FileName).Replace(' ', '_'), "_", 
                    string.Concat(Guid.NewGuid().ToString("N"), referenceKey));
                string fileExtension = Path.GetExtension(model.File.FileName);
                string name = string.Concat("_stg_", fileName, fileExtension).ToLower();

                string outPath = Path.Combine(_env.WebRootPath, "uploads/xyz_stegs/", name);
                encodedImage.Save(outPath, ImageFormat.Png);

                //save to db for download
                await dbContext.StegFiles.AddAsync(new StegStatelessFile
                {
                    UserId = user.userId,
                    FileName = Path.GetFileNameWithoutExtension(name),
                    ImagePath = $"uploads/xyz_stegs/{name}",
                    Reference = userReference,
                    ReferenceKey = referenceKey
                });
                await dbContext.SaveChangesAsync();

                response.Data = new EncryptMessageResponse(string.Concat("/uploads/xyz_stegs/", name));
                response.Message = "Message successfully embedded!";
                LogHelper.Log(StegAction.Encode.ToString(), response.Message, "Success", user.username!);
            }
            catch (Exception ex)
            {
                response.Message = "Failed to encode message";
                response.Status = false;
                LogHelper.Log(StegAction.Encode.ToString(), ex.Message, "Failed", user.username!);
            }

            //remove the original file
            if (File.Exists(originalPath)) File.Delete(originalPath);
            await TrackUserStats(StegAction.Encode, user.userId);
            return response;
        }

        public async Task<ResponseHandler<StegOutViewModel>> DecodeMessageAsync(DecodeViewModel model, (Guid userId, string username) user)
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
                LogHelper.Log(StegAction.Decode.ToString(), response.Message, "Success", user.username!);
            }
            catch (Exception ex)
            {
                response.Message = "Failed to extract message";
                response.Status = false;
                LogHelper.Log(StegAction.Decode.ToString(), ex.Message, "Failed", user.username!);
            }

            await TrackUserStats(StegAction.Decode, user.userId);
            return response;
        }

        public async Task<IEnumerable<StegStatelessFile>> GetAllStegFilesAsync(Guid userId)
        {
            return await dbContext.StegFiles.Where(f => f.UserId.Equals(userId)).OrderByDescending(f => f.CreatedAt)
                .AsNoTracking().ToListAsync();
        }
        public async Task<StegStatelessFile> GetStegFileAsync(Guid id)
            => (await dbContext.StegFiles.FirstOrDefaultAsync(f => f.Id.Equals(id)))!;

        public async Task<ResponseHandler> DeleteStegFileAsync(Guid id)
        {
            var response = new ResponseHandler();
            try
            {
                var file = await GetStegFileAsync(id) ?? throw new Exception("Invalid steg file. Or does not exist");
                string filePath = Path.Combine(_env.WebRootPath, file.ImagePath!);
                if (File.Exists(filePath)) File.Delete(filePath);
                dbContext.StegFiles.Remove(file);
                await dbContext.SaveChangesAsync();

                response.Status = true;
                response.Message = "Steg file deleted successfully!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }
        private async Task TrackUserStats(StegAction action, Guid userId)
        {
            await dbContext.UserStats.AddAsync(new Anaconda.Models.UserStat
            {
                UserId = userId,
                Action = action,
                LastAction = action.ToString(),
                LastActionTime = DateTime.Now
            });
            await dbContext.SaveChangesAsync();
        }
    }
}