using Anaconda.DataLayer;
using Anaconda.Helpers;
using Anaconda.UserViewResponse;
using Microsoft.EntityFrameworkCore;
using Steganography.ViewModels;

namespace Steganography.Services
{
    public class AdminViewService(ServiceDbContext dbContext) : IAdminViewService
    { 
		private readonly ServiceDbContext dbContext = dbContext;
        public async Task<ResponseHandler<DashboardDataViewModel>> DashboardDataAsync()
        {
			var response = new ResponseHandler<DashboardDataViewModel>();

            try
			{
				var users = await dbContext.Users.ToListAsync();
				var visitors = await dbContext.VisitationInfos.ToListAsync();
                var allLogs = LogHelper.GetLogs().ToArray();

                response.Data = new DashboardDataViewModel
				{
					TotalUsers = users.Count,
					TotalVisitors = visitors.Count,
					TotalLogs = allLogs.Length,
				};
			}
			catch (Exception ex)
			{
				response.Status = false;
				response.Message = ex.Message;
			}
			
			return response;
        }
        public async Task<ResponseHandler<IEnumerable<DashboardUserResponse>>> DashboardUsersAsync()
        {
			var response = new ResponseHandler<IEnumerable<DashboardUserResponse>>();

            try
			{
				var users = await dbContext.Users.OrderByDescending(u => u.CreatedAt).Select(u => new DashboardUserResponse
				{
					Id = u.Id,
					Email = u.Email,
					Username = u.UserName,
					PhoneNumber = u.PhoneNumber,
					VerificationSentAt = u.VerificationSentAt!,
					EmailConfirmed = u.EmailConfirmed,
					CreatedAt = u.CreatedAt,
				}).ToListAsync();

				response.Data = users;
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
