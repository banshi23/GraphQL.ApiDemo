using GraphQL.ApiDemo.Models.Users;
using GraphQL.ApiDemo.Repositories;
using HotChocolate.Authorization;

namespace GraphQL.ApiDemo.GraphQL.Mutations
{
	public class Mutations
	{
		[Authorize]
		public async Task<UserProfile> CreateUser([Service] IUserService userService, CreateOrUpdateUserInput createUserInput)
		{
			try
			{
				var user = await userService.CreateOrUpdateUserAsync(createUserInput);

				return user;
			}
			catch (Exception ex)
			{
				throw new GraphQLException(new Error(ex.Message));
			}
		}
		[Authorize]
		public async Task<bool> DeleteUserAsync([Service] IUserService userService, int id)
		{
			return await userService.DeleteUserAsync(id);
		}
	}
}
