using System.Text.Json.Serialization;

namespace Anaconda.UserViewResponse.ServiceResponses
{
    public class UserGeolocationResponse
    {
        [JsonPropertyName("country")]
        public string? Country {  get; set; }

        [JsonPropertyName("regionName")]
        public string? RegionName {  get; set; }

        [JsonPropertyName("city")]
        public string? City {  get; set; }

        [JsonPropertyName("isp")]
        public string? ISP {  get; set; }

        [JsonPropertyName("query")]
        public string? Query { get; set; }
    }
}