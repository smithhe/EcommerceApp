using Ecommerce.Shared.Dtos;
using System;
using System.Collections.Generic;

namespace Ecommerce.Shared.Responses.Order
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a request to get all Orders for a User
	/// </summary>
	public class GetAllOrdersByUserIdResponse : BaseResponse
	{
		/// <summary>
		/// A collection of <see cref="OrderDto"/> entities if any exist
		/// </summary>
		public IEnumerable<OrderDto> Orders { get; set; } = Array.Empty<OrderDto>();
	}
}