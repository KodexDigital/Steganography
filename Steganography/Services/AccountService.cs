using Anaconda.DataLayer;
using Anaconda.Helpers;
using Anaconda.Models;
using Anaconda.Requests;
using Anaconda.Settings;
using Anaconda.UserViewResponse;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Steganography.Services
{
    public class AccountService(ServiceDbContext dbContext, IEmailService emailService, UserManager<ApplicationUser> userManager,
        IOptionsSnapshot<SystemSettings> settings) : IAccountService
    {
        protected readonly ServiceDbContext dbContext = dbContext;
        protected readonly IEmailService emailService = emailService;
        protected readonly UserManager<ApplicationUser> userManager = userManager;
        protected readonly SystemSettings settings = settings.Value;
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
                    var regUser = await dbContext.Users.AddAsync(new ApplicationUser
                    {
                        Email = mail.Address,
                        UserName = mail.User,
                    });
                    var saved = await dbContext.SaveChangesAsync() > 0;
                    // send email verification link
                    if (saved)
                    {
                        var token = await GenerateEmailVerificationTokenAsync(regUser.Entity);
                        regUser.Entity.VerificationToken = token;
                        regUser.Entity.VerificationTokenExpires = DateTime.Now.AddMinutes(double.Parse(settings.TokenExpiresInMinutes!));
                        await userManager.UpdateAsync(regUser.Entity);

                        var callBackUrl = $"{settings.AccountVerificationPath}{WebUtility.HtmlEncode(token)}";
                        var sentMailResponse = await emailService.SendMailAsync(new DefaultSendMailRequest
                        {
                            Recipients =
                            [
                                new() { EmailAddress = mail.Address, Name = mail.User.ToUpper() }
                            ],
                            Subject = "Account Verification",
                            Body = $"Hi Stegian! <p>Please verify your account by clicking the link: <a href='{callBackUrl}'>Verify Account</a></p>"
                        });

                        if (sentMailResponse.Status)
                        {
                            var newUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(regUser.Entity.Id));
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

                    response.Message = "User registered successfully.";
                }

                LogHelper.Log("Account Registration", response.Message, "Success", email, null);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "An error occurred while registering the user";
                LogHelper.Log("Account Registration", $"{response.Message} - {ex.Message}", "Failed", email, null);
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
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == mail.Address.ToLower() && u.VerificationToken == generatedToken)
                    ?? throw new Exception("User not found or invalid user");

                if (DateTime.Now > user.VerificationTokenExpires)
                    throw new Exception("Verification token has expired");

                var result = await userManager.ConfirmEmailAsync(user, generatedToken);
                if ((result is not null && result.Succeeded))
                {
                    user.VerificationToken = null;
                    user.VerificationTokenExpires = null;
                    await userManager.UpdateAsync(user);
                    response.Message = "User verified successfully.";
                }
                else
                {
                    response.Message = "Failed to verify user. Please try again.";
                    response.Status = false;
                }
                LogHelper.Log("Account Verification", response.Message, response.Status ? "Success" : "Failed", email, null);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "An error occurred while verifying the user";
                LogHelper.Log("Account Verification", $"{response.Message} - {ex.Message}", "Failed", email, null);
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
                LogHelper.Log("Account Lock", response.Message, "Success", userId.ToString(), null);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"An error occurred while locking the user account: {ex.Message}";
                LogHelper.Log("Account Lock", $"{response.Message} - {ex.Message}", "Failed", userId.ToString(), null);
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
                    user.VerificationTokenExpires = DateTime.Now.AddMinutes(double.Parse(settings.TokenExpiresInMinutes!));
                    await userManager.UpdateAsync(user);

                    var callBackUrl = $"{settings.AccountVerificationPath}{WebUtility.HtmlEncode(token)}";
                    var sentMailResponse = await emailService.SendMailAsync(new DefaultSendMailRequest
                    {
                        Recipients =
                        [
                            new() { EmailAddress = mail.Address, Name = mail.User.ToUpper() }
                        ],
                        Subject = "Access Token",
                        Body = $"Hi Stegian! <p>Please click on the link to login to steg: <a href='{callBackUrl}'>Login Now ==></a></p>"
                    });

                    response.Status = sentMailResponse.Status;
                    response.Message = sentMailResponse.Status ? "Login link sent successfully." : "Failed to send login link.";
                }

                LogHelper.Log("Account Login", response.Message, response.Status ? "Success" : "Failed", user!.Id.ToString(), null);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "An error occurred while login in the user";
                LogHelper.Log("Account Login", $"{response.Message} - {ex.Message}", "Failed", userId.ToString(), null);
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

                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == mail.Address.ToLower() && u.VerificationToken == generatedToken)
                    ?? throw new Exception("User not found or invalid user");

                if (DateTime.Now > user.VerificationTokenExpires)
                    throw new Exception("Verification token has expired");
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "An error occurred while locking the user account";
                LogHelper.Log("Account Login Access", $"{response.Message} - {ex.Message}", "Failed", email.ToString(), null);
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
    }
}