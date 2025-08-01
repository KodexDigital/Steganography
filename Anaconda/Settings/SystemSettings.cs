namespace Anaconda.Settings
{
    public class SystemSettings
    {
        public string? TokenExpiresInMinutes { get; set; }
        public string? AccountVerificationPath { get; set; }
        public string? NotImportantTmpPass { get; set; }
        public string? DefaultEmailHeader { get; set; }
        public string? DonationPaymentLink { get; set; }
        public string? DefaultAdmin { get; set; }
        public string? DefaultAdminPassword { get; set; }
        public string? DefaultAdminRole { get; set; }
        public ZohoMailSettings? ZohoMailSettings { get; set; }
    }

    public class ZohoMailSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
    }
}