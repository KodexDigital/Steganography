using Anaconda.Common;
using Anaconda.DataLayer;
using Anaconda.Helpers;
using Anaconda.Models;
using Anaconda.Requests;
using Anaconda.Settings;
using Anaconda.UserViewResponse;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Steganography.ViewModels;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Steganography.Services
{
    public class AccountService(ServiceDbContext dbContext, IEmailService emailService, UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, IOptionsSnapshot<SystemSettings> settings,
        IRazorViewToStringRenderer renderer) : IAccountService
    {
        protected readonly ServiceDbContext dbContext = dbContext;
        protected readonly IEmailService emailService = emailService;
        protected readonly UserManager<ApplicationUser> userManager = userManager;
        protected readonly SignInManager<ApplicationUser> signInManager = signInManager;
        protected readonly SystemSettings settings = settings.Value;
        protected readonly IRazorViewToStringRenderer renderer = renderer;
        public async Task<ResponseHandler> RegisterUserAsync(string email)
        {
            var response = new ResponseHandler();
            try
            {
                var mail = new MailAddress(email);
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == mail.Address.ToLower());
                if (user is not null)
                {
                    response.Message = "User already exists.";
                    response.Status = false;
                }
                else
                {
                    var regUser = new ApplicationUser
                    {
                        Email = mail.Address,
                        UserName = mail.User,
                        PasswordHash = settings.NotImportantTmpPass,
                    };
                    
                    var result = await userManager.CreateAsync(regUser, regUser.PasswordHash!);
                    if(result is not null || result!.Succeeded)
                    {
                        // send email verification link
                        if (result.Succeeded)
                            await GenerateRegLinkAndSendAsync(response, mail, regUser);

                        response.Message = "User registered successfully.";
                    }
                    else
                    {
                        response.Message = "Failed to register user. Please try again.";
                        response.Status = false;
                    }
                }

                LogHelper.Log("Account Registration", response.Message, "Success", email);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                LogHelper.Log("Account Registration", ex.Message, "Failed", email);
            }
            return response;
        }

        public async Task<ResponseHandler> VerifyUserAsync(string verificationToken)
        {
            var response = new ResponseHandler();
            string email = string.Empty;
            try
            {
                TokenExtrator(verificationToken, out email, out string generatedToken);
                var mail = new MailAddress(email);
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == mail.Address.ToLower() && u.VerificationToken == verificationToken)
                    ?? throw new Exception("User not found or invalid user");

                if (DateTime.Now > user.VerificationTokenExpires)
                {
                    await GenerateRegLinkAndSendAsync(response, mail, user);
                    throw new Exception("Verification token has expired. Please check your email for the newly generated link");
                }

                var result = await userManager.ConfirmEmailAsync(user, generatedToken);
                if ((result is not null && result.Succeeded))
                {
                    user.VerificationToken = null;
                    user.VerificationTokenExpires = null;
                    user.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);

                    await signInManager.SignInAsync(user, isPersistent: true);
                    response.Message = "User logged in successfully.";
                }
                else
                {
                    response.Message = "Failed to verify user. Please try again.";
                    response.Status = false;
                }
                LogHelper.Log("Account Verification", response.Message, response.Status ? "Success" : "Failed", email);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                LogHelper.Log("Account Verification", ex.Message, "Failed", email);
            }

            return response;
        }

        public async Task<ResponseHandler> LockUserAccountAsync(Guid userId)
        {
            var response = new ResponseHandler();
            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
                if (user is null)
                {
                    response.Message = "User not found.";
                    response.Status = false;
                }
                else
                {
                    user.IsLocked = true;
                    dbContext.Users.Update(user);
                    await dbContext.SaveChangesAsync();
                    response.Message = "User account locked successfully.";
                }
                LogHelper.Log("Account Lock", response.Message, "Success", userId.ToString());
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                LogHelper.Log("Account Lock", response.Message, "Failed", userId.ToString());
            }
            return response;
        }

        public async Task<ResponseHandler> LoginAsync(string email)
        {
            var response = new ResponseHandler();
            Guid userId = Guid.Empty;
            try
            {
                var mail = new MailAddress(email);
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == mail.Address.ToLower());
                userId = user?.Id ?? Guid.Empty;
                if (user is null)
                {
                    response.Message = "Invalid user or does not exist";
                    response.Status = false;
                }
                else
                {
                    var token = await GenerateEmailVerificationTokenAsync(user);
                    user.VerificationToken = token;
                    user.VerificationTokenExpires = DateTime.Now.AddMinutes(double.Parse(settings.TokenExpiresInMinutes!)/2);
                    await userManager.UpdateAsync(user);

                    var model = new EmailViewModel
                    {
                        Subject = $"Access Token - {DateTime.Now:yyyyMdHHmssff}",
                        Title = "Welcome to K-Steg!",
                        BodyContent = "Please click on the link to login to Steg",
                        ActionUrl = $"{settings.AccountVerificationPath}{WebUtility.HtmlEncode(token)}",
                        ActionText = "Login Now"
                    };
                    var sentMailResponse = await emailService.SendMailAsync(new DefaultSendMailRequest
                    {
                        Recipients =
                        [
                            new() { EmailAddress = mail.Address, Name = mail.User.ToUpper() }
                        ],
                        Subject = model.Subject,
                        Body = OnTheGoEmailTemplating(model)
                        //Body = $"Hi Stegian! <p>Please click on the link to login to steg: <a href='{callBackUrl}'>Login Now ==></a></p>"
                    }, settings.DefaultEmailHeader!);

                    response.Status = sentMailResponse.Status;
                    response.Message = sentMailResponse.Status ? "Login link sent successfully. Check your email (Inbox or Junk folder)." : "Failed to send login link.";
                }

                LogHelper.Log("Account Login", response.Message, response.Status ? "Success" : "Failed", user?.Id.ToString() ?? string.Empty);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                LogHelper.Log("Account Login", response.Message, "Failed", userId.ToString());
            }
            return response;
        }

        public async Task<ResponseHandler> LoginAccessAsync(string accessToken)
        {
            var response = new ResponseHandler();
            string email = string.Empty;
            try
            {
                TokenExtrator(accessToken, out email, out string generatedToken);
                var mail = new MailAddress(email);

                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == mail.Address.ToLower() && u.VerificationToken == accessToken)
                    ?? throw new Exception("User not found or invalid user");

                if (DateTime.Now > user.VerificationTokenExpires)
                    throw new Exception("Verification token has expired");

                await signInManager.SignInAsync(user, isPersistent: true);
                response.Message = "User logged in successfully.";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                LogHelper.Log("Account Login Access", response.Message, "Failed", email.ToString());
            }
            return response;
        }

        public async Task<ResponseHandler> UserAccountValidationAsync(string token)
        {
            var response = new ResponseHandler();
            string email = string.Empty;

            TokenExtrator(token, out email, out string generatedToken);
            var mail = new MailAddress(email);
            var user = await userManager.FindByEmailAsync(mail.Address) ?? throw new Exception("Invalid user or does not exist");
            if (user.EmailConfirmed)
                response = await LoginAccessAsync(token);
            else
                response = await VerifyUserAsync(token);

            return response;
        }

        public async Task<ResponseHandler> UserLoginAsync(LoginViewModel model)
        {
            var response = new ResponseHandler();
            try
            {
                var user = await userManager.FindByEmailAsync(model.Username) ?? throw new Exception("Invalid user or user does not exist");
                var loginResult = await signInManager.PasswordSignInAsync(user, model.Password, true, false);
                if (loginResult.Succeeded) response.Message = "Login successful";
                else
                {
                    response.Status = false;
                    response.Message = "Login failed";
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }
        public async Task<ResponseHandler> LogoutAsync()
        {
            var response = new ResponseHandler();
            try
            {
                await signInManager.SignOutAsync();
                response.Message = "User logged out successfully.";
                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }
        protected async Task<string> GenerateEmailVerificationTokenAsync(ApplicationUser identityUser)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            var tokenData = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(String.Concat(identityUser.Email, ":", token)));
            return WebUtility.HtmlEncode(tokenData);
        }

        protected static void TokenExtrator(string verificationToken, out string email, out string generatedToken)
        {
            WebUtility.HtmlDecode(verificationToken);
            var token = WebEncoders.Base64UrlDecode(verificationToken!);
            string decodedTokenData = Encoding.UTF8.GetString(token);
            var splitToken = decodedTokenData.Split(':');
            email = splitToken.FirstOrDefault()!;
            generatedToken = splitToken.LastOrDefault()!;
        }
        protected async Task GenerateRegLinkAndSendAsync(ResponseHandler response, MailAddress mail, ApplicationUser regUser)
        {
            var token = await GenerateEmailVerificationTokenAsync(regUser);
            regUser.VerificationToken = token;
            regUser.VerificationTokenExpires = DateTime.Now.AddMinutes(double.Parse(settings.TokenExpiresInMinutes!));
            await userManager.UpdateAsync(regUser);

            var model = new EmailViewModel
            {
                Subject = $"Email Account Verification - {DateTime.Now:yyyyMdHHmssff}",
                Title = "Welcome to K-Steg!",
                BodyContent = "Click the button below to verify your account.",
                ActionUrl = $"{settings.AccountVerificationPath}{WebUtility.HtmlEncode(token)}",
                ActionText = "Verify Account"
            };
            var sentMailResponse = await emailService.SendMailAsync(new DefaultSendMailRequest
            {
                Recipients =
                [
                    new() { EmailAddress = mail.Address, Name = mail.User.ToUpper() }
                ],
                Subject = model.Subject,
                Body = OnTheGoEmailTemplating(model)
                //Body = $"<h5>Hi Stegian!</h5> Click the button below to verify your account. <br> {settings.AccountVerificationPath}{WebUtility.HtmlEncode(token)}"
            }, settings.DefaultEmailHeader!);

            if (sentMailResponse.Status)
            {
                var newUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(regUser.Id));
                if (newUser is not null)
                {
                    newUser.VerificationSentAt = DateTime.Now;
                    dbContext.Users.Update(newUser);
                    await dbContext.SaveChangesAsync();
                }
                response.Message = "Verification email sent successfully.";
            }
            else
            {
                response.Message = "Failed to send verification email.";
                response.Status = false;
            }
        }
        protected static string OnTheGoEmailTemplating(EmailViewModel model)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"UTF-8\">");
            sb.AppendLine("<meta charset=\"utf-8\" />");
            sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />");
            sb.AppendLine("<title>{{Subject}}</title>");
            sb.AppendLine("<link rel=\"icon\" type=\"image/x-icon\" href=\"~/icons/stegtool.ico\" />");
            sb.AppendLine("<link rel=\"apple-touch-icon\" href=\"~/icons/stegtool.jpg\">");
            sb.AppendLine("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");
            sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1, shrink-to-fit=no\">");
            sb.AppendLine("<meta name=\"description\" content=\"\">");
            sb.AppendLine("<meta name=\"author\" content=\"\">");
            sb.AppendLine("<link href=\"https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css\" rel=\"stylesheet\" type=\"text/css\" />");
            sb.AppendLine("</head>");
            sb.AppendLine("<body style=\"margin:0; padding:0; font-family:'Segoe UI', sans-serif; background-color:#f4f4f4;\">");

            sb.AppendLine("<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"background-color:#f4f4f4; padding: 40px 0;\">");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td align=\"center\">");

            sb.AppendLine("<table width=\"600\" cellpadding=\"0\" cellspacing=\"0\" style=\"background-color:#ffffff; border-radius:8px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.1);\">");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td style=\"background-color:#0d6efd; padding: 20px; color:#ffffff; text-align:center;\">");
            sb.AppendLine($"<h2 style=\"margin:0;\">{Constants.APP_NAME}</h2>");
            sb.AppendLine("<p style=\"margin:0; font-size: 14px;\">Secure Image Messaging Tool</p>");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td style=\"padding: 30px;\">");
            sb.AppendLine("<h1 style=\"margin-top:0;\">{{Title}}</h2>");
            sb.AppendLine("<p style=\"font-size: 20px; line-height: 1.5; color:#333333; padding-top: 10px;\">");
            sb.AppendLine("<h2>Hi {{RecipientName}},</h3>");
            sb.AppendLine("<span class=\"ml-4\" style=\"font-size: 24x;\">\">{{BodyContent}}</span>");
            sb.AppendLine("</p>");

            sb.AppendLine("<div style=\"margin-top: 30px; text-align: center;\">");
            sb.AppendLine("<a href=\"{{ActionLink}}\" target=\"_blank\" style=\"background-color:#0d6efd; color:#ffffff; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold;\">{{LinkText}}</a>");
            sb.AppendLine("</div>");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td style=\"padding: 20px; text-align: center; font-size: 12px; color:#999999;\">");
            sb.AppendLine("<hr>");
            sb.AppendLine("Copyright &copy; {{Year}} | <a href=\"#\" target=\"_blank\">Kodex Power Service</a>. All rights reserved.<br />");
            sb.AppendLine("<a href=\"https://securesteg.app/home/privacy\" class=\"me-3\" target=\"_blank\" style=\"color:#999999; text-decoration:underline;\">Privacy Policy</a>");
            sb.AppendLine("<a href=\"https://securesteg.app/home/terms\" class=\"me-3\" target=\"_blank\" style=\"color:#999999; text-decoration:underline;\">Terms of Service</a>");
            sb.AppendLine("<a href=\"https://securesteg.app/home/cookies\" target=\"_blank\" style=\"color:#999999; text-decoration:underline;\">Cookie Policy</a>");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("</table>");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            string emailHtml = sb.ToString();

            emailHtml = emailHtml.Replace("{{Subject}}", model.Subject)
                .Replace("{{Title}}", model.Title)
                .Replace("{{RecipientName}}", "Stegian")
                .Replace("{{BodyContent}}", model.BodyContent)
                .Replace("{{ActionLink}}", model.ActionUrl)
                .Replace("{{LinkText}}", model.ActionText)
                .Replace("{{Year}}", DateTime.Now.Year.ToString());

            return emailHtml;
        }
    }
}