using GraphQL.ApiDemo.Data;
using GraphQL.ApiDemo.Models.Users;

namespace GraphQL.ApiDemo.Repositories
{
	public class UserRoleService : IUserRoleService
	{
		private readonly DbContextClass _dbContext;

		public UserRoleService(DbContextClass dbContext)
		{
			_dbContext = dbContext;
		}

		public IList<UserRole> GetRoleById(int id)
		{
			return _dbContext.Roles.Where(_ => _.UserId == id).ToList();
		}
	}

}
