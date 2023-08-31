using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities
{
	public class OrderItem : AuditableEntity
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
	}
}