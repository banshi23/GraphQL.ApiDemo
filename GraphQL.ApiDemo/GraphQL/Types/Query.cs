using GraphQL.ApiDemo.Entities;
using GraphQL.ApiDemo.Repositories;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
namespace GraphQL.ApiDemo.GraphQL.Types
{
	public class Query
	{
		//[UsePaging(IncludeTotalCount = true)]
		//[UseProjection]
		//[UseSorting]
		//[UseFiltering]
		[Authorize]
		public async Task<List<ProductDetails>> GetProductListAsync([Service] IProductService productService)
		{
			return await productService.ProductListAsync();
		}
		//[UseFirstOrDefault]
		[Authorize]
		public async Task<ProductDetails> GetProductDetailsByIdAsync([Service] IProductService productService, Guid productId)
		{
			return await productService.GetProductDetailByIdAsync(productId);
		}

		[GraphQLDeprecated("This query is deprecated.")]
		public async Task<List<ProductDetails>> GetDepricatedProductListAsync([Service] IProductService productService)
		{
			return await productService.ProductListAsync();
		}
	}
}
