using Ecommerce.Domain.Common;
using Ecommerce.Shared.Enums;
using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities
{
	public class Order : AuditableEntity
	{
		public int Id { get; set; }
		
		public Guid UserId { get; set; }
		
		public OrderStatus Status { get; set; }
		
		public double Total { get; set; }
		
		public Guid PayPalRequestId { get; set; }
		
		public IEnumerable<OrderItem> OrderItems { get; set; } = null!;
	}
}