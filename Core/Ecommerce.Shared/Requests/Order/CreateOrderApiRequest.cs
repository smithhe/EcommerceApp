using System.Collections.Generic;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Enums;

namespace Ecommerce.Shared.Requests.Order
{
	/// <summary>
	/// A Api request to create a new Order
	/// </summary>
	public class CreateOrderApiRequest
	{
		/// <summary>
		/// The items in the cart to create the order from
		/// </summary>
		public IEnumerable<CartItemDto>? CartItems { get; set; }
		
		/// <summary>
		/// The method of payment for the order
		/// </summary>
		public PaymentSource PaymentSource { get; set; }
	}
}