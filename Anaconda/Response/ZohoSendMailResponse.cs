namespace Anaconda.Response
{
    public class ZohoSendMailResponse
    {
        public List<Datum>? data { get; set; }
        public string? message { get; set; }
        public string? request_id { get; set; }
        public string? @object { get; set; }
    }

    public class Datum
    {
        public string? code { get; set; }
        public List<object>? additional_info { get; set; }
        public string? message { get; set; }
    }
}
