namespace GraphQL.ApiDemo.Models
{
	public class TokenPayload
	{
		public string Message { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
		public TokenPayload(string message, string accessToken, string refreshToken)
		{
			Message = message;
			AccessToken = accessToken;
			RefreshToken = refreshToken;
		}
	}
}
