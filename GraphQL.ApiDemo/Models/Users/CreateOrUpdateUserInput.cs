using System.ComponentModel.DataAnnotations;

namespace GraphQL.ApiDemo.Models.Users
{
	public class CreateOrUpdateUserInput
	{
		public int UserId { get; set; }
		public string? UserName { get; set; }
		public string? Name { get; set; }
		public string? PhoneNumber { get; set; }
		public string? CompanyName { get; set; }
		public string? CompanyType { get; set; }
		public string? Position { get; set; }
		public string? ApprovalStatus { get; set; }
		public string? ActivationStatus { get; set; }
		public string? EmailId { get; set; }
		public string? CustomerType { get; set; }
		public int? CountryId { get; set; }
		public bool? MultiFactorActive { get; set; }

		[StringLength(50)]
		public string? DefaultPage { get; set; }

		[StringLength(150)]
		public string? ExternalUserId { get; set; }
		public int? DefaultLandingPageId { get; set; }

		[StringLength(100)]
		public string? Commodity { get; set; }
		[Required]
		[StringLength(10)]
		public string LandingPageChangedBy { get; set; }
	}
}
