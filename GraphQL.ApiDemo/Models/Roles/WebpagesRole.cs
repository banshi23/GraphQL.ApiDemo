using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GraphQL.ApiDemo.Models.Roles
{
	[Table("webpages_Roles")]
	public class WebpagesRole
	{
		[Key]
		public int RoleId { get; set; }
		[Required]
		[MaxLength(256)]
		public string RoleName { get; set; }
		public int? RolePriority { get; set; }
		[MaxLength(15)]
		public string? CRMInvoiceSource { get; set; }
		public bool? IsBulkVisible { get; set; }
		public bool? IsEDIMapping { get; set; }
		public bool? IsBulkExceptionVisible { get; set; }
		[MaxLength(15)]
		public string? AllowCOT { get; set; }
		public bool? IsGroupPermissionForBilling { get; set; }
		[Required]
		public int RoleCategoryId { get; set; } = 0;
		[Required]
		public bool IsCreatedByUtilidex { get; set; } = false;
	}
}
