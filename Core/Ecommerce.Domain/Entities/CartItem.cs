using Ecommerce.Domain.Common;
using System;

namespace Ecommerce.Domain.Entities
{
	public class CartItem : AuditableEntity
	{
		public int Id { get; set; }
		
		public int ProductId { get; set; }
		
		public Guid UserId { get; set; }
		
		public int Quantity { get; set; }
	}
}