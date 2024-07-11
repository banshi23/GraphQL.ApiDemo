using System.ComponentModel.DataAnnotations.Schema;

namespace GraphQL.ApiDemo.Models.Users
{
	[Table("Users")]
	public class User
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string EmailAddress { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenExpiration { get; set; }
	}
}
