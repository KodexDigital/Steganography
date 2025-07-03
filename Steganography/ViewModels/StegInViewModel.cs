using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Steganography.ViewModels
{
    public class StegInViewModel
    {
        [DataType(DataType.Upload)]
        [Required] public IFormFile? Image { get; set; }
        [Required] public string? Message { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Steg Key must be at least 8 characters long.")]
        [Required] public string? StegKey { get; set; }
    }
}