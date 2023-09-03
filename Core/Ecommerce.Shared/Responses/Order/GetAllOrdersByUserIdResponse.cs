using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Order;
using System;
using System.Collections.Generic;

namespace Ecommerce.Shared.Responses.Order
{
	/// <summary>
	/// A implementation of <see cref="BaseResponse" /> for a <see cref="GetAllOrdersByUserIdApiRequest"/>
	/// </summary>
	public class GetAllOrdersByUserIdResponse : BaseResponse
	{
		/// <summary>
		/// A collection of <see cref="OrderDto"/> entities if any exist
		/// </summary>
		public IEnumerable<OrderDto> Orders { get; set; } = Array.Empty<OrderDto>();
	}
}