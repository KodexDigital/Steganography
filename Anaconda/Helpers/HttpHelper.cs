using Anaconda.Enums;
using Anaconda.Extensions;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace Anaconda.Helpers
{
    public static class HttpHelper
    {
        public static async Task<HttpResponseMessage> SendRequest(object request, string baseAddress, string requestUri, HttpMethod method, Dictionary<string, string>? headers = null,
            ApiRequestContentType requestContentType = ApiRequestContentType.application_json)
        {
            try
            {
                using var client = new HttpClient();
                string contentType = MediaTypeNames.Application.Json;
                switch (requestContentType)
                {
                    case ApiRequestContentType.application_json:
                        contentType = MediaTypeNames.Application.Json;
                        break;
                    case ApiRequestContentType.text_plain:
                        contentType = "text/plain";
                        break;
                    case ApiRequestContentType.x_www_form_url_encoded:
                        contentType = "application/x-www-form-urlencoded";
                        break;
                    case ApiRequestContentType.multipart_form_data:
                        contentType = "multipart/form-data";
                        break;
                    default:
                        break;
                }

                client.BaseAddress = new System.Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (headers is not null)
                    foreach (KeyValuePair<string, string> header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);

                if (method.Equals(HttpMethod.Post) || method.Equals(HttpMethod.Put) || method.Equals(HttpMethod.Patch))
                {
                    HttpContent? content = null;
                    if (requestContentType.Equals(ApiRequestContentType.x_www_form_url_encoded))
                        content = new FormUrlEncodedContent(request.AsEnumerableKeyValuePair());
                    else
                    {
                        string data = JsonConvert.SerializeObject(request);
                        content = new StringContent(data, Encoding.UTF8, contentType);
                    }
                    return await client.PostAsync(requestUri, content);
                }
                else if (method.Equals(HttpMethod.Delete))
                    return await client.DeleteAsync(requestUri);
                else if (method.Equals(HttpMethod.Get))
                    return await client.GetAsync(requestUri);

                return null!;
            }
            catch
            {
                throw;
            }
        }
    }
}
