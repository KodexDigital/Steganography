using Anaconda.Enums;

namespace Anaconda.Models
{
    public class UserStat : BaseModel
    {
        public int TotalActions { get; set; }
        public StegAction Action { get; set; }
        public string? LastAction { get; set; }
        public DateTime? LastActionTime { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}