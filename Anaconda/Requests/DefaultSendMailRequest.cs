using Microsoft.AspNetCore.Http;

namespace Anaconda.Requests
{
    public class DefaultSendMailRequest
    {
        public List<DefaultRecipient>? Recipients { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public IFormFile? Attachment { get; set; }
    }

    public class DefaultRecipient
    {
        public string? EmailAddress { get; set; }
        public string? Name { get; set; }
    }
}
