using System;

namespace Ecommerce.Shared.Dtos
{
	public class CartItemDto
	{
		public int Id { get; set; }
		
		public int ProductId { get; set; }
		
		public Guid UserId { get; set; }
		
		public int Quantity { get; set; }
	}
}