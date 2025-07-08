namespace Steganography.ViewModels
{
    public class UserStatsViewModel
    {
        public int TotalActions { get; set; }
        public int EncodeCount { get; set; }
        public int EncodeSuccessCount { get; set; }
        public int EncodeFailedCount { get; set; }
        public int DecodeCount { get; set; }
        public int DecodeSuccessCount { get; set; }
        public int DecodeFailedCount { get; set; }
        public string? LastAction { get; set; }
        public DateTime? LastActionTime { get; set; }
        public string? Username { get; set; }
    }
}
