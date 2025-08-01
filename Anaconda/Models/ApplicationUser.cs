using Microsoft.AspNetCore.Identity;

namespace Anaconda.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime? VerificationSentAt { get; set; }
        public bool IsLocked { get; set; } = false;
        public string? VerificationToken { get; set; }
        public DateTime? VerificationTokenExpires { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<UserStat> UserStats { get; set; }
        public ApplicationUser()
        {
            UserStats = [];
            CreatedAt = DateTime.Now;
        }
    }
}