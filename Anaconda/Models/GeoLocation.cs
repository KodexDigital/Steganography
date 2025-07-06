namespace Anaconda.Models
{
    public class GeoLocation : BaseModel
    {
        public string? Country { get; set; }
        public string? RegionName { get; set; }
        public string? City { get; set; }
        public string? Isp { get; set; }
        public string? Query { get; set; } // IP address
        public virtual ICollection<VisitationInfo>? VisitationInfos { get; set; }   
        public GeoLocation()
        {
            VisitationInfos = [];
        }
    }
}