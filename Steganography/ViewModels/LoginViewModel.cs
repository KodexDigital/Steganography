using System.ComponentModel.DataAnnotations;

namespace Steganography.ViewModels
{
    public record LoginViewModel([Required] string Username, [Required] string Password);
}
