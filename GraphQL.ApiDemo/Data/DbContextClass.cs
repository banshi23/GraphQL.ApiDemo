using GraphQL.ApiDemo.Entities;
using GraphQL.ApiDemo.Models.Roles;
using GraphQL.ApiDemo.Models.Users;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GraphQL.ApiDemo.Data
{
	public class DbContextClass : DbContext
	{
		public DbContextClass(DbContextOptions<DbContextClass> options) : base(options)
		{
		}
		public DbSet<ProductDetails> Products { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<UserRole> Roles { get; set; }
	}
}
