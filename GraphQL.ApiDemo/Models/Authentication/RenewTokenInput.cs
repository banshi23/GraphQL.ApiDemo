namespace GraphQL.ApiDemo.Models.Authentication
{
	public class RenewTokenInput
	{
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
	}
}
