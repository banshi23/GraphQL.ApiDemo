using GraphQL.ApiDemo.Data;
using GraphQL.ApiDemo.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.ApiDemo.Repositories
{
	public class UserService : IUserService
	{
		private readonly DbContextClass _dbContext;

		public UserService(DbContextClass dbContext)
		{
			_dbContext = dbContext;
		}

		public IExecutable<UserProfile> GetUserById([ID(null)] int id)
		{
			return _dbContext.UserProfile.Where(x => x.UserId == id).AsExecutable();
		}
		public IExecutable<UserProfile> GetUser()
		{
			return _dbContext.UserProfile.AsExecutable();
		}
		public async Task<UserProfile> CreateOrUpdateUserAsync(CreateOrUpdateUserInput input)
		{
			if(input.UserId > 0)
			{
				var getUser = await _dbContext.UserProfile.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == input.UserId);
				if (getUser == null)
				{
					throw new GraphQLException(new Error("User not found.", "USER_NOT_FOUND"));
				}
				else
				{
					var updateUserRequest = new UserProfile
					{
						UserId = getUser.UserId,
						UserName =  getUser.UserName,
						Name = input.Name ?? getUser.Name,
						PhoneNumber = input.PhoneNumber ?? getUser.PhoneNumber,
						CompanyName = input.CompanyName ?? getUser.CompanyName,
						CompanyType = input.CompanyType ?? getUser.CompanyType,
						Position = input.Position ?? getUser.Position,
						ApprovalStatus = input.ApprovalStatus ?? getUser.ApprovalStatus,
						ActivationStatus = input.ActivationStatus ?? getUser.ActivationStatus,
						EmailId = input.EmailId ?? getUser.EmailId,
						CustomerType = input.CustomerType ?? getUser.CustomerType,
						CountryId = input.CountryId ?? getUser.CountryId,
						MultiFactorActive = input.MultiFactorActive ?? getUser.MultiFactorActive,
						DefaultPage = input.DefaultPage ?? getUser.DefaultPage,
						ExternalUserId = input.ExternalUserId ?? getUser.ExternalUserId,
						DefaultLandingPageId = input.DefaultLandingPageId ?? getUser.DefaultLandingPageId,
						Commodity = input.Commodity ?? getUser.Commodity,
						GroupPermissionLastUpdateDateTime = DateTime.UtcNow,
						BasketPermissionLastUpdateDateTime = DateTime.UtcNow,
						LandingPageChangedBy = input.LandingPageChangedBy ?? getUser.LandingPageChangedBy,
						UpdatedDateTime = DateTime.UtcNow
					};
					_dbContext.Update(updateUserRequest);
					await _dbContext.SaveChangesAsync();
					return updateUserRequest;
				}
			}
			else
			{
				var newUser = new UserProfile
				{
					UserName = Guid.NewGuid().ToString(),
					Name = input.Name,
					PhoneNumber = input.PhoneNumber,
					CompanyName = input.CompanyName,
					CompanyType = input.CompanyType,
					Position = input.Position,
					ApprovalStatus = input.ApprovalStatus,
					ActivationStatus = input.ActivationStatus,
					EmailId = input.EmailId,
					CustomerType = input.CustomerType,
					CountryId = input.CountryId,
					MultiFactorActive = input.MultiFactorActive,
					DefaultPage = input.DefaultPage,
					ExternalUserId = input.ExternalUserId,
					DefaultLandingPageId = input.DefaultLandingPageId,
					Commodity = input.Commodity,
					GroupPermissionLastUpdateDateTime = DateTime.UtcNow,
					BasketPermissionLastUpdateDateTime = DateTime.UtcNow,
					LandingPageChangedBy = input.LandingPageChangedBy,
					UpdatedDateTime = DateTime.UtcNow
				};
				await _dbContext.UserProfile.AddAsync(newUser);

				await _dbContext.SaveChangesAsync();
				return newUser;
			}
		}
		public async Task<bool> DeleteUserAsync(int id)
		{
			var findUser = await _dbContext.UserProfile.AsNoTracking().FirstOrDefaultAsync(_ => _.UserId == id);
			if (findUser != null)
			{
				_dbContext.UserProfile.Remove(findUser);
				var result = await _dbContext.SaveChangesAsync();
				if (result > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				throw new GraphQLException(new Error("User not found."));
			}
		}
	}
}
