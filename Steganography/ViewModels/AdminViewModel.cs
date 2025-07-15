namespace Steganography.ViewModels
{
    public class AdminViewModel
    {
        public DashboardDataViewModel? DashboardData { get; set; }
        public IEnumerable<DashboardUserResponse>? Users { get; set; }
    }
}