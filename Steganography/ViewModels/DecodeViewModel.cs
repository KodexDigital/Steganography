using System.ComponentModel.DataAnnotations;

namespace Steganography.ViewModels
{
    public class DecodeViewModel
    {
        [DataType(DataType.Upload)]
        [Required] public IFormFile? StegFile { get; set; }

        [DataType(DataType.Password)]
        [Required] public string? StegPassKey { get; set; }
        public string? ExtractedMessage { get; set; }
    }
}