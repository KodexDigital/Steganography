namespace Anaconda.Models
{
    public class ApplicationUser : BaseModel
    {
        public string? EmailAddress { get; set; }
        public virtual ICollection<UserStat> UserStats { get; set; }
        public ApplicationUser()
        {
            UserStats = [];
        }
    }
}