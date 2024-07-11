using GraphQL.ApiDemo.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.ApiDemo.Data
{
	public class DbContextClass : DbContext
	{
		public DbContextClass(DbContextOptions<DbContextClass> options) : base(options)
		{
		}
		public DbSet<UserProfile> UserProfile { get; set; }
	}
}
