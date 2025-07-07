using System.ComponentModel.DataAnnotations;

namespace Steganography.ViewModels
{
    public class AuthenticationViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required] public string? Email { get; set; }   
    }
}