using GraphQL.ApiDemo.Models.Users;

namespace GraphQL.ApiDemo.Repositories
{
	public interface IUserService
	{
		IExecutable<UserProfile> GetUser();
		IExecutable<UserProfile> GetUserById([ID] int id);
		Task<UserProfile> CreateOrUpdateUserAsync(CreateOrUpdateUserInput input);
		Task<bool> DeleteUserAsync(int id);
	}
}
