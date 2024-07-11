namespace GraphQL.ApiDemo.Models.Users
{
	public class CreateUserInput
	{
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
	}
}
