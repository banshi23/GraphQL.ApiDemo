using GraphQL.ApiDemo.Models;
using GraphQL.ApiDemo.Models.Authentication;
using GraphQL.ApiDemo.Models.Users;

namespace GraphQL.ApiDemo.Repositories
{
	public interface IUserService
	{
		Task<User> CreateUserAsync(CreateUserInput createUserSettingInput);
		IExecutable<User> GetUser();
		IExecutable<User> GetUserById([ID] int id);
		TokenPayload Login(LoginInput loginInput);
		TokenPayload RenewAccessToken(RenewTokenInput renewTokenInput);
	}
}
