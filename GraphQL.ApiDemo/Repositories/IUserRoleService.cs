using GraphQL.ApiDemo.Models.Users;

namespace GraphQL.ApiDemo.Repositories
{
	public interface IUserRoleService
	{
		IList<UserRole> GetRoleById(int id);
	}
}
