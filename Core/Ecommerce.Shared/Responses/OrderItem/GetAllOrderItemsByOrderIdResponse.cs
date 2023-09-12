using Ecommerce.Shared.Dtos;
using System;
using System.Collections.Generic;

namespace Ecommerce.Shared.Responses.OrderItem
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to get all OrderItems for a Order
	/// </summary>
	public class GetAllOrderItemsByOrderIdResponse : BaseResponse
	{
		/// <summary>
		/// The collection of Order Items if any exist for the order
		/// </summary>
		public IEnumerable<OrderItemDto> OrderItems { get; set; } = Array.Empty<OrderItemDto>();
	}
}