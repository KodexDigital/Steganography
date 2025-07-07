using Anaconda.Enums;
using Anaconda.Helpers;
using Anaconda.Requests;
using Anaconda.Response;
using Anaconda.UserViewResponse;
using Newtonsoft.Json;

namespace Steganography.Services
{
    public class EmailService : IEmailService
    {
        public async Task<ResponseHandler> SendMailAsync(DefaultSendMailRequest request, string authHeaderKey)
        {
            var response = new ResponseHandler();
            var responseData = new ZohoSendMailResponse();
            try
            {
                string requestUrl = "/v1.1/email";
                var req = new ZohoSendMailRequest
                {
                    from = new From
                    {
                        address = "support@kodextechnology.com",
                        name = "KIPS Steg Tool",
                    },
                    to = request.Recipients?.Select(r => new Anaconda.Requests.To
                    {
                        email_address = new EmailAddress
                        {
                            address = r.EmailAddress,
                            name = r.Name
                        }
                    }).ToList(),
                    subject = request.Subject,
                    htmlbody = request.Body
                };

                var headerAuth = new Dictionary<string, string>
                {
                    {"Authorization", authHeaderKey}
                };

                var httpResponse = await HttpHelper.SendRequest(req, "https://api.zeptomail.com", requestUrl, HttpMethod.Post, headerAuth, ApiRequestContentType.application_json);
                if (httpResponse is not null && httpResponse.IsSuccessStatusCode)
                {
                    responseData = JsonConvert.DeserializeObject<ZohoSendMailResponse>(await httpResponse.Content.ReadAsStringAsync());
                    if (responseData is not null || responseData!.data is not null) response.Message = "Email sent successfully";
                    else throw new ApplicationException("Unable to send email");
                }
                else throw new Exception("Error sending email");
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
