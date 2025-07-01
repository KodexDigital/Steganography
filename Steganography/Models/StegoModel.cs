using Microsoft.AspNetCore.Http;

namespace StegoWebApp.Models
{
    public class StegoModel
    {
        public IFormFile ImageFile { get; set; }
        public string SecretMessage { get; set; }
        public string ExtractedMessage { get; set; }
    }
}