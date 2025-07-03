namespace Steganography.ViewModels
{
    public class EncodeViewModel
    {
        public IFormFile? File { get; set; }
        public string? SecretMessage { get; set; }
        public string? StegPassKey { get; set; }
    }
}