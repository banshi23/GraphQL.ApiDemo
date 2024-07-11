using System.ComponentModel.DataAnnotations.Schema;

namespace GraphQL.ApiDemo.Models.Users
{
	[Table("UserRoles")]
	public class UserRole
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string Name { get; set; }
	}
}
