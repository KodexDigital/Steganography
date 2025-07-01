namespace Steganography.ViewModels
{
    public class StegoViewModel
    {
        public IFormFile ImageFile { get; set; }
        public string Message { get; set; }
        public string Password { get; set; }
        public string ExtractedMessage { get; set; }
    }

}
