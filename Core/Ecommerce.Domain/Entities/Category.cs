using System.ComponentModel.DataAnnotations;
using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities
{
	public class Category : AuditableEntity
	{
		public int Id { get; set; }
		
		[MaxLength(50)]
		public string Name { get; set; } = null!;
		
		[MaxLength(200)]
		public string Summary { get; set; } = null!;
	}
}