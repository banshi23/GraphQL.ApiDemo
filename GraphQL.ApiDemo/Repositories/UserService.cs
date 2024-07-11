using GraphQL.ApiDemo.Data;
using GraphQL.ApiDemo.Models;
using GraphQL.ApiDemo.Models.Authentication;
using GraphQL.ApiDemo.Models.Users;
using GraphQL.ApiDemo.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GraphQL.ApiDemo.Repositories
{
	public class UserService : IUserService
	{
		private readonly DbContextClass _dbContext;
		private readonly JwtSettings _jwtSettings;
		private readonly IUserRoleService _userRoleRepository;

		public UserService(DbContextClass dbContext, IOptions<JwtSettings> jwtSettings, IUserRoleService userRoleRepository)
		{
			_dbContext = dbContext;
			_userRoleRepository = userRoleRepository;
			_jwtSettings = jwtSettings.Value;
		}

		public IExecutable<User> GetUserById([ID(null)] int id)
		{
			return _dbContext.Users.Where(x => x.Id == id).AsExecutable();
		}
		public IExecutable<User> GetUser()
		{
			return _dbContext.Users.AsExecutable();
		}
		public async Task<User> CreateUserAsync(CreateUserInput createUserSettingInput)
		{
			var item = new User
			{
				EmailAddress = createUserSettingInput.EmailAddress,
				UserName = createUserSettingInput?.UserName,
				Password = HashPassword(createUserSettingInput.Password),
			};
			_dbContext.Users.Add(item);
			await _dbContext.SaveChangesAsync();
			return item;
		}

		public TokenPayload Login(LoginInput loginInput)
		{
			string Message = "Success";
			if (string.IsNullOrEmpty(loginInput.UserName)
			|| string.IsNullOrEmpty(loginInput.Password))
			{
				Message = "Invalid Credentials";
				return new TokenPayload(Message, "", "");
			}
			var user = _dbContext.Users.FirstOrDefault(x => x.UserName == loginInput.UserName);
			if (user == null)
			{
				Message = "Invalid Credentials";
				return new TokenPayload(Message, "", "");
			}

			if (!ValidatePasswordHash(loginInput.Password, user.Password))
			{
				Message = "Invalid Credentials";
				return new TokenPayload(Message, "", "");
			}
			var roles = _userRoleRepository.GetRoleById(user.Id);

			var userTokenPayload = new TokenPayload(Message, GenerateToken(user, roles), GenerateRefreshToken());

			user.RefreshToken = userTokenPayload.RefreshToken;
			user.RefreshTokenExpiration = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpMinute);

			UpdateRefreshToken(user);

			return userTokenPayload;
		}
		public TokenPayload RenewAccessToken(RenewTokenInput renewTokenInput)
		{
			string Message = "Success";
			if (string.IsNullOrEmpty(renewTokenInput.AccessToken)
			|| string.IsNullOrEmpty(renewTokenInput.RefreshToken))
			{
				Message = "Invalid Token";
				return new TokenPayload(Message, "", "");
			}

			ClaimsPrincipal principal = GetClaimsFromExpiredToken(renewTokenInput.AccessToken);

			if (principal == null)
			{
				Message = "Invalid Token";
				return new TokenPayload(Message, "", "");
			}

			string userName = principal.Claims.Where(_ => _.Type == "UserName").Select(_ => _.Value).FirstOrDefault();
			if (string.IsNullOrEmpty(userName))
			{
				Message = "Invalid Token";
				return new TokenPayload(Message, "", "");
			}

			var user = _dbContext.Users.FirstOrDefault(x => x.UserName == userName && x.RefreshToken == renewTokenInput.RefreshToken && x.RefreshTokenExpiration > DateTime.Now);
			if (user == null)
			{
				Message = "Invalid Token";
				return new TokenPayload(Message, "", "");
			}

			var userRoles = _userRoleRepository.GetRoleById(user.Id);

			var userTokenPayload = new TokenPayload(Message, GenerateToken(user, userRoles), GenerateRefreshToken());

			user.RefreshToken = userTokenPayload.RefreshToken;
			user.RefreshTokenExpiration = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpMinute);

			UpdateRefreshToken(user);

			return userTokenPayload;
		}

		private bool UpdateRefreshToken(User user)
		{
			var existingUser =  _dbContext.Users.Find(user.Id);
			if (existingUser == null)
			{
				return false;
			}

			existingUser.RefreshToken = user.RefreshToken;
			existingUser.RefreshTokenExpiration = user.RefreshTokenExpiration;

			_dbContext.Users.Update(existingUser);
			var result =  _dbContext.SaveChanges();

			return result > 0;
		}

		private bool ValidatePasswordHash(string password, string dbPassword)
		{
			byte[] hashBytes = Convert.FromBase64String(dbPassword);

			byte[] salt = new byte[16];
			Array.Copy(hashBytes, 0, salt, 0, 16);

			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
			byte[] hash = pbkdf2.GetBytes(20);

			for (int i = 0; i < 20; i++)
			{
				if (hashBytes[i + 16] != hash[i])
				{
					return true;
				}
			}

			return true;
		}

		private string HashPassword(string password)
		{
			byte[] salt;
			new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
			byte[] hash = pbkdf2.GetBytes(20);

			byte[] hashBytes = new byte[36];
			Array.Copy(salt, 0, hashBytes, 0, 16);
			Array.Copy(hash, 0, hashBytes, 16, 20);

			return Convert.ToBase64String(hashBytes);
		}
		private string GenerateToken(User user, IList<UserRole> roles)
		{
			var securtityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
			var credentials = new SigningCredentials(securtityKey, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>
		{
			new Claim("UserName", user.UserName),
			new Claim("Email", user.EmailAddress)
		};
			if ((roles?.Count ?? 0) > 0 && roles != null)
			{
				foreach (var role in roles)
					claims.Add(new Claim(ClaimTypes.Role, role.Name));
			}

			var jwtSecurityToken = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpMinute),
				signingCredentials: credentials,
				claims: claims
			);
			return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
		}
		private string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using (var generator = RandomNumberGenerator.Create())
			{
				generator.GetBytes(randomNumber);
				return Convert.ToBase64String(randomNumber);
			}
		}
		private ClaimsPrincipal GetClaimsFromExpiredToken(string accessToken)
		{
			var tokenValidationParameter = new TokenValidationParameters
			{
				ValidIssuer = _jwtSettings.Issuer,
				ValidateIssuer = true,
				ValidAudience = _jwtSettings.Audience,
				ValidateAudience = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
				ValidateLifetime = false // ignore expiration
			};

			var jwtHandler = new JwtSecurityTokenHandler();
			var principal = jwtHandler.ValidateToken(accessToken, tokenValidationParameter, out SecurityToken securityToken);

			var jwtScurityToken = securityToken as JwtSecurityToken;
			if (jwtScurityToken == null)
			{
				return null;
			}

			return principal;
		}
	}
}
