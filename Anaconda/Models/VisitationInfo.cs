namespace Anaconda.Models
{
    public class VisitationInfo : BaseModel
    {
        public string? IpAddress { get; set; }
        public string? Browser { get; set; }
        public string? OperatingSystem { get; set; }
        public string? Device { get; set; }
        public virtual Guid LocationId { get; set; }
        public virtual GeoLocation? Location { get; set; }
    }
}