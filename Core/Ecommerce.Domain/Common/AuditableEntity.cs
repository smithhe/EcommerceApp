using System;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.Common
{
	public abstract class AuditableEntity
	{
		[MaxLength(255)]
		[Required]
		public string CreatedBy { get; set; } = null!;
		
		[Required]
		public DateTime CreatedDate { get; set; }
		
		[MaxLength(255)]
		public string? LastModifiedBy { get; set; }
		
		public DateTime? LastModifiedDate { get; set; }
	}
}