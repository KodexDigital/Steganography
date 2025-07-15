namespace Steganography.ViewModels
{
    public class DashboardUserResponse
    {
        public Guid Id {  get; set; }
        public string? Username {  get; set; }
        public string? Email {  get; set; }
        public string? PhoneNumber {  get; set; }
        public bool EmailConfirmed {  get; set; }
        public DateTime? VerificationSentAt {  get; set; }
        public DateTime CreatedAt {  get; set; }
    }
}