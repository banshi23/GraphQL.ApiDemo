using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraphQL.ApiDemo.Entities
{
	[Table("ProductDetails")]

	public class ProductDetails
	{
		[Key]
		[GraphQLNonNullType]
		public Guid Id { get; set; }
		public string ProductName { get; set; }
		public string ProductDescription { get; set; }
		public int ProductPrice { get; set; }
		public int ProductStock { get; set; }
	}
}
