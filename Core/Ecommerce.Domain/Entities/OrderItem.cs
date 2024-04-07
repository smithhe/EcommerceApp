using System.ComponentModel.DataAnnotations;
using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities
{
	public class OrderItem : AuditableEntity
	{
		public int Id { get; set; }
		
		public int OrderId { get; set; }
		
		[MaxLength(255)]
		public string ProductName { get; set; } = null!;
		
		[MaxLength(255)]
		public string ProductDescription { get; set; } = null!;
		
		[MaxLength(255)]
		public string ProductSku { get; set; } = null!;
		
		public int Quantity { get; set; }
		
		public double Price { get; set; }
	}
}