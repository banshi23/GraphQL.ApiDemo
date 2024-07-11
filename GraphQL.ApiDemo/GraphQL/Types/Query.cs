using GraphQL.ApiDemo.Models.Users;
using GraphQL.ApiDemo.Repositories;
using HotChocolate.Authorization;
namespace GraphQL.ApiDemo.GraphQL.Types
{
	public class Query
	{
		[Authorize]
		[UsePaging]
		public IExecutable<UserProfile> GetUsers([Service] IUserService userService) => userService.GetUser();

		[Authorize]
		public IExecutable<UserProfile> GetUserById([Service] IUserService userService, [ID] int id) => userService.GetUserById(id);
	}
}
