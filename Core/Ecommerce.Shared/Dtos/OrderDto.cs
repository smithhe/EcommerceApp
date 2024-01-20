using Ecommerce.Shared.Enums;
using System;
using System.Collections.Generic;

namespace Ecommerce.Shared.Dtos
{
	public class OrderDto
	{
		public int Id { get; set; }
		public Guid UserId { get; set; }
		public DateTime CreatedDate { get; set; }
		public OrderStatus Status { get; set; }
		public double Total { get; set; }
		public IEnumerable<OrderItemDto> OrderItems { get; set; } = null!;
	}
}