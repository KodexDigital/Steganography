namespace Anaconda.Models
{
    public class UserStat : BaseModel
    {
        public int TotalActions { get; set; }
        public int EncodeCount { get; set; }
        public int DecodeCount { get; set; }
        public string? LastAction { get; set; }
        public DateTime? LastActionTime { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}