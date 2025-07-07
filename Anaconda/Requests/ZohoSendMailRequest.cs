namespace Anaconda.Requests
{
    public class ZohoSendMailRequest
    {
        public From? from { get; set; }
        public List<To>? to { get; set; }
        public string? subject { get; set; }
        public string? htmlbody { get; set; }
        public List<Attachment>? attachments { get; set; }
    }

    public class EmailAddress
    {
        public string? address { get; set; }
        public string? name { get; set; }
    }

    public class From
    {
        public string? address { get; set; }
        public string? name { get; set; }
    }
    public class To
    {
        public EmailAddress? email_address { get; set; }
    }
    public class Attachment
    {
        public string? name { get; set; }
        public string? mime_type { get; set; }
        public string? content { get; set; }
    }
}
