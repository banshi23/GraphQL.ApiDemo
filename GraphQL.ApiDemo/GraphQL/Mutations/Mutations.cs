using GraphQL.ApiDemo.Entities;
using GraphQL.ApiDemo.Models;
using GraphQL.ApiDemo.Models.Authentication;
using GraphQL.ApiDemo.Models.Users;
using GraphQL.ApiDemo.Repositories;
using HotChocolate.Authorization;


namespace GraphQL.ApiDemo.GraphQL.Mutations
{
	public class Mutations
	{
		[Authorize]
		public async Task<bool> AddProductAsync([Service] IProductService productService, ProductDetails productDetails)
		{
			return await productService.AddProductAsync(productDetails);
		}
		[Authorize]
		public async Task<bool> UpdateProductAsync([Service] IProductService productService, ProductDetails productDetails)
		{
			return await productService.UpdateProductAsync(productDetails);
		}
		[Authorize]
		public async Task<bool> DeleteProductAsync([Service] IProductService productService, Guid productId)
		{
			return await productService.DeleteProductAsync(productId);
		}

		[Authorize]
		public async Task<User> CreateUser([Service] IUserService userService, CreateUserInput createUserInput)
		{
			try
			{
				var user = await userService.CreateUserAsync(createUserInput);

				return user;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		public TokenPayload Login([Service] IUserService userService, LoginInput loginInput)
		{
			return userService.Login(loginInput);
		}
		public TokenPayload RenewAccessToken([Service] IUserService userService, RenewTokenInput renewTokenInput)
		{
			return userService.RenewAccessToken(renewTokenInput);
		}
	}
}
